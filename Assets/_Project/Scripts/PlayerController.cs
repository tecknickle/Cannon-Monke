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
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.01f;
        [SerializeField] float jumpCooldown = 0f;

        const float Zerof = 0f;

        Transform mainCamera;

        float velocity;
        float currentSpeed;
        float jumpVelocity;

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        

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
            timers = new(2) { jumpTimer, jumpCooldownTimer };

            jumpTimer.OnTimerStart += () => jumpVelocity = jumpForce;
            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        void Start() => inputReader.EnablePlayerActions();

        void OnEnable()
        {
            inputReader.Jump += OnJump;
        }

        void OnDisable()
        {
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

        void Update()
        {
            movement = new (
                inputReader.Direction.x, 
                0f, 
                inputReader.Direction.y);

            HandleTimers();
            UpdateAnimation();
        }

        void FixedUpdate()
        {
            HandleMovement();
            HandleJump();            
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

        void HandleJump()
        {
            // If not jumping or grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.isGrounded)
            {
                jumpVelocity = Zerof;
                return;
            }

            // If jumping or falling velocity
            if (!jumpTimer.IsRunning)
            {
                // Gravity takes over
                jumpVelocity += Physics.gravity.y * Time.fixedDeltaTime;
            }

            // Apply velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }

        void HandleMovement()
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

        void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime);
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
