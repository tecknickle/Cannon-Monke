using KBCore.Refs;
using System;
using Unity.Cinemachine;
using UnityEngine;

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

        [Header("Settings")]
        [SerializeField] float moveSpeed = 300f;
        [SerializeField] float rotationSpeed = 600f;
        [SerializeField] float smoothTime = 0.2f;

        const float Zerof = 0f;

        float velocity;
        float currentSpeed;

        Vector3 movement;

        Transform mainCamera;

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
        }

        void Start() => inputReader.EnablePlayerActions();

        void Update()
        {
            movement = new (
                inputReader.Direction.x, 
                0f, 
                inputReader.Direction.y);

            UpdateAnimation();
        }

        void FixedUpdate()
        {
            // handlejump()
            HandleMovement();
        }

        void UpdateAnimation()
        {
            animator.SetFloat(Speed, currentSpeed);
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
