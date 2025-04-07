using KBCore.Refs;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        public static CameraManager Instance { get; private set; }

        [Header("References")]
        [SerializeField, Anywhere] Transform defaultTarget;
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField, Anywhere] CinemachineOrbitalFollow cinemachineCameraOrbital;
        [SerializeField, Anywhere] CinemachineCamera cinemachineCamera;

        [Header("Camera Control Settings")]
        [SerializeField, Range(0, 5)] float cameraSensitivity = 1f;

        Transform currentTarget;
        bool cameraMovementLock;

        public Transform CurrentTarget => currentTarget;

        void OnEnable()
        {
            inputReader.Look += OnLook;
        }

        void OnDisable()
        {
            inputReader.Look -= OnLook;
        }

        void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            // Set initial target
            if (defaultTarget != null)
            {
                SetTarget(defaultTarget);
            }
            else
            {
                Debug.LogError("Default target not set in CameraManager.");
            }
        }

        void Start()
        {
            // Lock cursor to center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (cameraMovementLock) return;

            // If device is mouse use fixedDeltaTime, else use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // Set camera axis values
            cinemachineCameraOrbital.HorizontalAxis.Value += 
                cameraMovement.x * cameraSensitivity * deviceMultiplier;

            cinemachineCameraOrbital.VerticalAxis.Value -= 
                cameraMovement.y * cameraSensitivity * deviceMultiplier;
        }

        public void SetTarget(Transform newTarget)
        {
            if (newTarget != null)
            {
                currentTarget = newTarget;
                cinemachineCamera.Follow = newTarget;
                cinemachineCamera.LookAt = newTarget;
            }
        }

        public void ReturnToDefault()
        {
            SetTarget(defaultTarget);
        }

        public void ToggleBetweenDefaultAnd(Transform altTarget)
        {
            if (currentTarget == defaultTarget)
            {
                SetTarget(altTarget);
            }
            else
            {
                ReturnToDefault();
            }
        }

        public void ToggleBetweenCannonAndProjectile(Transform cannon, Transform projectile)
        {
            if (currentTarget == cannon)
            {
                SetTarget(projectile);
            }
            else if (currentTarget == projectile)
            {
                SetTarget(cannon);
            }
            else
            {
                // Default to cannon if neither is the current target
                SetTarget(cannon);
            }
        }
    }
}
