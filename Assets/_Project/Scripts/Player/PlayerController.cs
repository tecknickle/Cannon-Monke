using KBCore.Refs;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Animator animator;
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Interactor interactor;
        [SerializeField, Self] PlayerObjectHolding objectHolding;
        [SerializeField, Anywhere] CinemachineCamera cinemachineCamera;
        [SerializeField, Anywhere] InputReader inputReader;

        [Header("SO Events")]
        [SerializeField, Anywhere] VoidEventSO onPlayerSwitchCamera;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 300f;
        [SerializeField] float rotationSpeed = 600f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 20f;
        [SerializeField] float jumpCooldown = 0.5f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float maxFallVelocity = -30f;
        [SerializeField] float minFallVelocity = -3f;

        [Header("Throw Settings")]
        [SerializeField] public float throwForce = 10f;
        [SerializeField] float dropForce = 5f;
        [SerializeField] float throwCooldown = 0.3f;

        [Header("Push Settings")]
        [SerializeField] float pushForce = 10f;
        [SerializeField] float pushDistance = 2f;
        [SerializeField] float pushCooldown = 0.3f;

        [Header("Cannon Mode Settings")]
        [SerializeField] float cannonFireCooldown = 2f;
        [SerializeField] float cameraSwitchCooldown = 1f;

        [Header("Other Settings")]
        [SerializeField] float interactDuration = 5f;
        [SerializeField] float interactCooldown = 0.5f;
        [SerializeField] float emoteDuration = 8f;
        [SerializeField] float emoteCooldown = 0f;

        const float Zerof = 0f;

        Transform mainCamera;
        Transform playersActiveProjectile;

        float velocity;
        float currentSpeed;
        float verticalVelocity;

        Vector3 movement;
        Vector3 pushRayOffset = new(0f, 0.5f, 0f);

        bool IsInCannonMode;
        CannonController currentCannon;

        List<Timer> timers;

        CountdownTimer jumpTimer;

        CountdownTimer interactTimer;
        CountdownTimer interactCooldownTimer;

        CountdownTimer emoteTimer;
        CountdownTimer emoteCooldownTimer;

        CountdownTimer throwObjectTimer;
        CountdownTimer pushTimer;
        CountdownTimer cannonFireTimer;

        CountdownTimer cameraSwitchTimer;

        StateMachine stateMachine;

        public static event Action<bool> OnPlayerCameraSwitch;

        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            mainCamera = Camera.main.transform;
            IsInCannonMode = false;
            currentCannon = null;

            cinemachineCamera.Follow = transform;
            cinemachineCamera.LookAt = transform;

            // Invoke event when transform is teleported, adjusting cinemachine cam position accordingly
            cinemachineCamera.OnTargetObjectWarped(
                transform,
                transform.position - cinemachineCamera.transform.position - Vector3.forward);

            rb.freezeRotation = true;

            SetupTimers();
            SetupStatemachine();
        }

        void SetupTimers()
        {
            // Setup timers
            jumpTimer = new CountdownTimer(jumpCooldown);

            interactTimer = new CountdownTimer(interactDuration);
            interactCooldownTimer = new CountdownTimer(interactCooldown);

            emoteTimer = new CountdownTimer(emoteDuration);
            emoteCooldownTimer = new CountdownTimer(emoteCooldown);

            throwObjectTimer = new CountdownTimer(throwCooldown);
            pushTimer = new CountdownTimer(pushCooldown);
            cannonFireTimer = new CountdownTimer(cannonFireCooldown);
            cameraSwitchTimer = new CountdownTimer(cameraSwitchCooldown);

            timers = new(9) {
                jumpTimer,
                interactTimer,
                interactCooldownTimer,
                throwObjectTimer,
                emoteTimer,
                emoteCooldownTimer,
                pushTimer,
                cannonFireTimer,
                cameraSwitchTimer
            };

            jumpTimer.OnTimerStart += () => verticalVelocity = jumpForce;

            interactTimer.OnTimerStart += () => interactor.DoInteraction();

            interactTimer.OnTimerStop += () =>
            {
                if (objectHolding.IsHolding) objectHolding.DropObject(dropForce);
                interactCooldownTimer.Start();
            };

            emoteTimer.OnTimerStart += () => movement = Vector3.zero;
            emoteTimer.OnTimerStop += () => emoteCooldownTimer.Start();

            throwObjectTimer.OnTimerStart += () =>
            {
                movement = Vector3.zero;
                objectHolding.ThrowObject(throwForce);
            };

            pushTimer.OnTimerStart += () => movement = Vector3.zero;

            cannonFireTimer.OnTimerStart += () => currentCannon.PlayerFiringCannon();

            // Active projectile transform lives in Cannon Firing Handler
            cameraSwitchTimer.OnTimerStart += () =>
            {
                // If in cannon mode, camera will toggle between projectile/cannon
                // If not in cannon mode, toggle between projectile/player
                OnPlayerCameraSwitch?.Invoke(IsInCannonMode);
            };
        }
        
        void SetupStatemachine()
        {
            // Setup Statemachine
            stateMachine = new StateMachine();
            
            // Declare states
            var locomotionState = new LocomotionState(this, animator);
            var jumpState = new JumpState(this, animator);
            var fallingState = new FallingState(this, animator);
            var emoteState = new EmoteState(this, animator);
            var holdingLocomotionState = new HoldingLocomotionState(this, animator);
            var holdingThrowState = new HoldingThrowState(this, animator);
            var pushState = new PushState(this, animator);
            var cannonModeState = new CannonModeState(this, animator);

            // Define transitions
            At(locomotionState, jumpState,
                new FuncPredicate(() => jumpTimer.IsRunning));

            At(locomotionState, holdingLocomotionState,
                new FuncPredicate(() => objectHolding.IsHolding));

            At(holdingLocomotionState, holdingThrowState,
                new FuncPredicate(() => throwObjectTimer.IsRunning));
            
            At(locomotionState, pushState,
                new FuncPredicate(() => pushTimer.IsRunning));

            At(locomotionState, emoteState,
                new FuncPredicate(() => emoteTimer.IsRunning));

            Any(cannonModeState,
                new FuncPredicate(() => IsInCannonMode));

            Any(fallingState,
                new FuncPredicate(() => !groundChecker.IsGrounded
                && rb.linearVelocity.y < minFallVelocity));

            Any(locomotionState,
                new FuncPredicate(() => IsLocomotionState()));

            // Set initial state
            stateMachine.SetState(locomotionState);
        }

        bool IsLocomotionState()
        {
            if (groundChecker.IsGrounded
                && !jumpTimer.IsRunning
                && !emoteTimer.IsRunning
                && !throwObjectTimer.IsRunning
                && !objectHolding.IsHolding
                && !pushTimer.IsRunning
                && !IsInCannonMode
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void At(IState from, IState to, 
            IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        void Any(IState to, 
            IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Start() => inputReader.EnablePlayerActions();

        void OnEnable()
        {   
            inputReader.Fire += OnFire;
            inputReader.Interact += OnInteract;
            inputReader.Emote1 += OnEmote;
            inputReader.Jump += OnJump;
            inputReader.SwitchCamera += OnSwitchCamera;
        }

        void OnDisable()
        {
            inputReader.Fire -= OnFire;
            inputReader.Interact -= OnInteract;
            inputReader.Emote1 -= OnEmote;
            inputReader.Jump -= OnJump;
            inputReader.SwitchCamera -= OnSwitchCamera;
        }

        public void Push()
        {
            Ray ray = new(transform.position + pushRayOffset, transform.forward);
            
            if (Physics.Raycast(ray, out RaycastHit hitInfo, pushDistance))
            {
                //Debug.Log("Hit: " + hitInfo.collider.gameObject.name);
                if (hitInfo.collider.CompareTag("Crate") 
                    || hitInfo.collider.CompareTag("Banana")
                    || hitInfo.collider.CompareTag("Racoon")
                    )
                {
                    hitInfo.collider.GetComponent<Rigidbody>()
                        .AddForce(transform.forward * pushForce, ForceMode.Impulse);
                }
            }
        }

        void OnSwitchCamera(bool performed)
        {
            if (performed 
                && !cameraSwitchTimer.IsRunning
                && !cannonFireTimer.IsRunning)
            {
                cameraSwitchTimer.Start();
            }
        }
        void OnJump(bool performed)
        {
            if (performed
                && !jumpTimer.IsRunning
                && !emoteTimer.IsRunning
                && groundChecker.IsGrounded)
            {
                jumpTimer.Start();
            }
        }

        void OnInteract(bool performed)
        {
            if (performed 
                && !interactTimer.IsRunning
                && !interactCooldownTimer.IsRunning
                && groundChecker.IsGrounded)
            {
                interactTimer.Start();
            }
            else if (!performed && interactTimer.IsRunning)
            {
                interactTimer.Stop();
            }
            else if (interactTimer.IsRunning && !groundChecker.IsGrounded)
            {
                interactTimer.Stop();
            }
        }

        void OnFire(bool performed)
        {   
            if (performed
                && IsInCannonMode)
            {
                cannonFireTimer.Start();
            }
            else if (performed 
                && !objectHolding.IsHolding
                && !pushTimer.IsRunning)
            {
                pushTimer.Start();
            }
            else if (performed
                && !throwObjectTimer.IsRunning
                && objectHolding.IsHolding)
            {
                throwObjectTimer.Start();
            }
        }

        void OnEmote(bool performed)
        {
            // Emote only plays when holding emote key down
            if (performed
                && !emoteTimer.IsRunning
                && !emoteCooldownTimer.IsRunning
                && groundChecker.IsGrounded)
            {
                emoteTimer.Start();
            }
            else if (!performed && emoteTimer.IsRunning)
            {
                emoteTimer.Stop();
            }
        }

        void Update()
        {
            movement = new (
                inputReader.Direction.x, 
                0f, 
                inputReader.Direction.y);

            stateMachine.Update();

            HandleTimers();
            UpdateAnimation();
        }

        void FixedUpdate()
        {
            stateMachine.FixedUpdate();        
        }

        void UpdateAnimation()
        {
            animator.SetFloat(Speed, currentSpeed);
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        public void HandleGravity()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                verticalVelocity = Zerof;
                return;
            }
            // If jumping or falling velocity
            else
            {
                // Gravity takes over
                verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
                
                // Player has a terminal velocity when falling
                if (verticalVelocity < maxFallVelocity)
                {
                    verticalVelocity = maxFallVelocity;
                }
            }

            // Apply velocity
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x, 
                verticalVelocity, 
                rb.linearVelocity.z);
            // Debug.Log("vertical velocity: " + verticalVelocity);
        }

         public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * movement;
            
            if (adjustedDirection.magnitude > Zerof)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothDamp(adjustedDirection.magnitude);
            }
            else
            {
                SmoothDamp(Zerof);

                // Reset horizontal velocity for a snappy stop
                rb.linearVelocity = new(
                    Zerof, 
                    rb.linearVelocity.y, 
                    Zerof);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
            {
                // Move the player
                Vector3 velocity = moveSpeed * Time.fixedDeltaTime * adjustedDirection;

                rb.linearVelocity = new(
                    velocity.x, 
                    rb.linearVelocity.y, 
                    velocity.z);
            }

        void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime);
        }

        void SmoothDamp(float value)
        {
            currentSpeed = Mathf.SmoothDamp(
                currentSpeed, 
                value, 
                ref velocity, 
                smoothTime);
        }

        public bool PlayerCannonMode(bool state, CannonController cannon)
        {
            if (cannon != null)
            {
                rb.linearVelocity = Vector3.zero; // Stop player movement when entering cannon mode
                currentCannon = cannon;
                cannon.EnterCannonMode(inputReader);
                return IsInCannonMode = state;
            }
            else
            {
                currentCannon = null;
                return IsInCannonMode = false;
            }
        }
    }
}
