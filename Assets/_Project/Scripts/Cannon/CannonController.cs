using KBCore.Refs;
using System.Transactions;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class CannonController : ValidatedMonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField, Anywhere] InputReader inputReader;

        [Header("Settings")]
        [SerializeField] float horizontalRotationSpeed = 100f;
        [SerializeField] float verticalRotationSpeed = 100f;
        [SerializeField] Vector3 positionOffset = new(0f, -2f, -6f);

        PlayerController currentPlayer;
        CinemachineCamera currentCamera;

        void Awake()
        {
            currentPlayer = null;
            currentCamera = null;
        }

        public void HandleAimingRotation()
        {
            // no op
        }

        public void EnterCannonMode(CinemachineCamera camera)
        {
            if (currentPlayer != null)
            {
                currentPlayer.transform.SetPositionAndRotation(
                    transform.position + positionOffset,
                    transform.rotation);

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
            currentPlayer.PlayerEnterCannonMode(false, null);
            currentPlayer = null;
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
                    player.PlayerEnterCannonMode(true, this);
                }
            }
        }

        // Fire();
    }
}
