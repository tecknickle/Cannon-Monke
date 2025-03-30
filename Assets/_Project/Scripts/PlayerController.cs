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

        [Header("Other Settings")]
        [SerializeField] float emoteDuration = 1f;
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
        CountdownTimer emoteTimer;
        CountdownTimer emoteCooldownTimer;

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

            // Setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            emoteTimer = new CountdownTimer(emoteDuration);
            emoteCooldownTimer = new CountdownTimer(emoteCooldown);
            timers = new(4) { jumpTimer, jumpCooldownTimer, emoteTimer, emoteCooldownTimer };

            jumpTimer.OnTimerStart += () => verticalVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();

            emoteTimer.OnTimerStop += () => emoteCooldownTimer.Start();

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

            At(fallingState, locomotionState, 
                new FuncPredicate(() => groundChecker.isGrounded));

            At(emoteState, locomotionState,
                new FuncPredicate(() => rb.linearVelocity.magnitude > Zerof));

            Any(fallingState,
                new FuncPredicate(() => !groundChecker.isGrounded
                && rb.linearVelocity.y < minFallVelocity));

            Any(emoteState,
                new FuncPredicate(() => emoteTimer.IsRunning
                && groundChecker.isGrounded));

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
            // TODO: Interact 
            inputReader.Emote1 += OnEmote;
            inputReader.Jump += OnJump;
        }

        void OnDisable()
        {
            inputReader.Emote1 -= OnEmote;
            inputReader.Jump -= OnJump;
        }

        void OnJump(bool performed)
        {
            if (performed
                && !jumpTimer.IsRunning
                && !jumpCooldownTimer.IsRunning
                && groundChecker.isGrounded)
            {
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        void OnEmote(bool performed)
        {
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

        public void HandleJump()
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
