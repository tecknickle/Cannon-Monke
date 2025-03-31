using KBCore.Refs;
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CannonMonke
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere] InputReader inputReader;
        [SerializeField, Anywhere] CinemachineOrbitalFollow cinemachineCamera;

        [Header("Camera Control Settings")]
        [SerializeField, Range(0, 5)] float cameraSensitivity = 1f;

        bool cameraMovementLock;

        void OnEnable()
        {
            inputReader.Look += OnLook;
        }

        void OnDisable()
        {
            inputReader.Look -= OnLook;
        }

        private void Start()
        {
            // Lock cursor to center of the screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (cameraMovementLock) return;

            // If device is mouse use fixedDeltaTime, else use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime;

            // Set camera axis values
            cinemachineCamera.HorizontalAxis.Value += 
                cameraMovement.x * cameraSensitivity * deviceMultiplier;

            cinemachineCamera.VerticalAxis.Value -= 
                cameraMovement.y * cameraSensitivity * deviceMultiplier;
        }
    }
}
