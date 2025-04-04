using KBCore.Refs;
using System.Transactions;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class CannonController : ValidatedMonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField, Anywhere] Transform cannonBarrel;
        [SerializeField, Anywhere] Transform cannonBase;

        [Header("Aiming Settings")]
        [SerializeField] float xAxisRotationSpeed = 100f;
        [SerializeField] float yAxisRotationSpeed = 100f;
        [SerializeField] float interpolationSpeed = 20f;

        [Header("Clamping Settings")]
        [SerializeField] float upperLimitInDegrees = 1f;
        [SerializeField] float lowerLimitInDegrees = 60f;
        [SerializeField] float leftLimitInDegrees = -45f;
        [SerializeField] float rightLimitInDegrees = 45f;

        [Header("Other Settings")]
        [SerializeField] Vector3 positionOffset = new(0f, -2f, -6f);

        PlayerController currentPlayer;
        InputReader currentInputReader;
        CinemachineCamera currentCamera;

        const float Zerof = 0f;

        float yAxisRotation;
        float xAxisRotation;

        void Awake()
        {
            currentPlayer = null;
            currentInputReader = null;
            currentCamera = null;
        }

        void Update()
        {
            if (currentPlayer != null)
            {
                HandleAimingRotation();
            }
        }

        public void HandleAimingRotation()
        {
            float xAxisRotationInput = currentInputReader.Direction.y * xAxisRotationSpeed * Time.deltaTime;
            float yAxisRotationInput = currentInputReader.Direction.x * yAxisRotationSpeed * Time.deltaTime;

            yAxisRotation += yAxisRotationInput;
            yAxisRotation = Mathf.Clamp(
                yAxisRotation, 
                leftLimitInDegrees, 
                rightLimitInDegrees); // Clamp horizontal rotation

            xAxisRotation += xAxisRotationInput;
            xAxisRotation = Mathf.Clamp(
                xAxisRotation, 
                upperLimitInDegrees, 
                lowerLimitInDegrees); // Clamp vertical rotation


            cannonBarrel.transform.localRotation = Quaternion.Slerp(
                cannonBarrel.transform.localRotation,
                Quaternion.Euler(xAxisRotation, yAxisRotation, Zerof),
                interpolationSpeed * Time.deltaTime);

            cannonBase.transform.localRotation = Quaternion.Slerp(
                cannonBase.transform.localRotation,
                Quaternion.Euler(0f, yAxisRotation, Zerof), // Only rotate the base horizontally
                interpolationSpeed * Time.deltaTime);
        }

        // Fire();

        public void EnterCannonMode(CinemachineCamera camera, InputReader input)
        {
            if (currentPlayer != null)
            {
                currentPlayer.transform.SetPositionAndRotation(
                    transform.position + positionOffset,
                    transform.rotation);

                currentInputReader = input;

                currentCamera = camera;
                camera.LookAt = transform; // Set the camera to look at the cannon
                camera.Follow = transform;
            }
        }

        public void ExitCannonMode()
        {
            currentCamera.LookAt = currentPlayer.transform;
            currentCamera.Follow = currentPlayer.transform;
            currentCamera = null;

            currentInputReader = null;

            currentPlayer.PlayerCannonMode(false, null);
            currentPlayer = null;

            // reset cannon position/orientation to parents'
            xAxisRotation = Zerof; // Reset x-axis rotation
            yAxisRotation = Zerof; // Reset y-axis rotation
            cannonBarrel.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Reset barrel rotation to default
            cannonBase.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // Reset base rotation to default
        }

        public void Interact(Transform interactor)
        {
            if (interactor.TryGetComponent(out PlayerController player))
            {
                // If player is currently in cannon mode,
                // will exit cannon on second interaction
                if (currentPlayer == player)
                {
                    ExitCannonMode();
                }
                else
                {
                    currentPlayer = player;
                    player.PlayerCannonMode(true, this);
                }
            }
        }
    }
}
