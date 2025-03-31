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

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 300f;
        [SerializeField] float rotationSpeed = 600f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 20f;
        [SerializeField] float jumpDuration = 1f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float gravityMultiplier = 3f;
        [SerializeField] float maxFallVelocity = -30f;
        [SerializeField] float minFallVelocity = -3f;

        [Header("Throw Settings")]
        [SerializeField] float throwDuration = 1f;
        [SerializeField] float throwCooldown = 0f;

        [Header("Other Settings")]
        [SerializeField] float interactDuration = 5f;
        [SerializeField] float interactCooldown = 1f;
        [SerializeField] float emoteDuration = 8f;
        [SerializeField] float emoteCooldown = 0f;

        const float Zerof = 0f;

        Transform mainCamera;

        float velocity;
        float currentSpeed;
        float verticalVelocity;

        Vector3 movement;

        List<Timer> timers;

        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;

        CountdownTimer interactTimer;
        CountdownTimer interactCooldownTimer;

        CountdownTimer emoteTimer;
        CountdownTimer emoteCooldownTimer;

        CountdownTimer throwObjectTimer;
        CountdownTimer throwObjectCooldownTimer;

        StateMachine stateMachine;
        
        // Animator parameters
        static readonly int Speed = Animator.StringToHash("Speed");

        void Awake()
        {
            mainCamera = Camera.main.transform;

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
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);

            interactTimer = new CountdownTimer(interactDuration);
            interactCooldownTimer = new CountdownTimer(interactCooldown);

            emoteTimer = new CountdownTimer(emoteDuration);
            emoteCooldownTimer = new CountdownTimer(emoteCooldown);

            throwObjectTimer = new CountdownTimer(throwDuration);
            throwObjectCooldownTimer = new CountdownTimer(throwCooldown);

            timers = new(8) {
                jumpTimer,
                jumpCooldownTimer,
                interactTimer,
                interactCooldownTimer,
                throwObjectTimer,
                throwObjectCooldownTimer,
                emoteTimer,
                emoteCooldownTimer,
            };

            jumpTimer.OnTimerStart += () => verticalVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            interactTimer.OnTimerStart += () => interactor.DoInteraction();
            interactTimer.OnTimerStop += () => objectHolding.DropObject();
            interactTimer.OnTimerStop += () => interactCooldownTimer.Start();

            throwObjectTimer.OnTimerStart += () => objectHolding.ThrowObject();
            throwObjectTimer.OnTimerStop += () => throwObjectCooldownTimer.Start();

            emoteTimer.OnTimerStart += () => movement = Vector3.zero;
            emoteTimer.OnTimerStop += () => emoteCooldownTimer.Start();
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

            // Define transitions
            At(locomotionState, jumpState,
                new FuncPredicate(() => jumpTimer.IsRunning));

            Any(fallingState,
                new FuncPredicate(() => !groundChecker.isGrounded
                && rb.linearVelocity.y < minFallVelocity));

            At(locomotionState, emoteState,
                new FuncPredicate(() => emoteTimer.IsRunning
                && groundChecker.isGrounded));

            Any(locomotionState,
                new FuncPredicate(() => groundChecker.isGrounded
                && !jumpTimer.IsRunning
                && !emoteTimer.IsRunning));

            // Set initial state
            stateMachine.SetState(locomotionState);
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
        }

        void OnDisable()
        {
            inputReader.Fire -= OnFire;
            inputReader.Interact -= OnInteract;
            inputReader.Emote1 -= OnEmote;
            inputReader.Jump -= OnJump;
        }

        void OnJump(bool performed)
        {
            if (performed
                && !jumpTimer.IsRunning
                && !jumpCooldownTimer.IsRunning
                && !emoteTimer.IsRunning
                && groundChecker.isGrounded)
            {
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        void OnInteract(bool performed)
        {
            if (performed 
                && !interactTimer.IsRunning
                && !interactCooldownTimer.IsRunning)
            {
                interactTimer.Start();
            }
            else if (!performed && interactTimer.IsRunning)
            {
                interactTimer.Stop();
            }
        }

        void OnFire(bool performed)
        {
            if (performed
                && !throwObjectTimer.IsRunning
                && !throwObjectCooldownTimer.IsRunning)
            {
                throwObjectTimer.Start();
            }
            else if (!performed && throwObjectTimer.IsRunning)
            {
                throwObjectTimer.Stop();
            }
        }

        void OnEmote(bool performed)
        {
            // Emote only plays when holding emote key down
            if (performed
                && !emoteTimer.IsRunning
                && !emoteCooldownTimer.IsRunning
                && groundChecker.isGrounded)
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
            if (!jumpTimer.IsRunning && groundChecker.isGrounded)
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
            Debug.Log("vertical velocity: " + verticalVelocity);
        }

         public void HandleMovement()
        {
            var movementDirection = new Vector3(
                inputReader.Direction.x, 
                0f, 
                inputReader.Direction.y).normalized;

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
                Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;

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
    }
}
