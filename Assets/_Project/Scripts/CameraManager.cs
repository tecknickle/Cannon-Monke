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
        [SerializeField, Anywhere] CinemachineCamera cinemachineCamera;

        [Header("Settings")]
        //[SerializeField, Range(0.5f, 30f)] float speedMultiplier = 1.0f;

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

        private void OnLook(Vector2 cameraMovement)
        {
            if (cameraMovementLock) return;

            //cinemachineCamera.XAxis.InputAxisValue = cameraMovement.x * speedMultiplier;
            //cinemachineCamera.YAxis.InputAxisValue = cameraMovement.y * speedMultiplier;
        }
    }
}
