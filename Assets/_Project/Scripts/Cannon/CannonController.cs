using KBCore.Refs;
using System.Collections;
using System.Threading;
using System.Transactions;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class CannonController : ValidatedMonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField, Self] CannonFiringHandler firingHandler;
        [SerializeField, Self] CannonLoadingHandler loadingHandler;
        [SerializeField, Self] CannonAimingHandler aimingHandler;
        [SerializeField, Self] CinemachineImpulseSource impulseSource;
        [SerializeField, Anywhere] Transform cannonCameraTarget;

        [Header("Settings")]
        [SerializeField] Vector3 positionOffset = new(0f, -2f, -6f);

        PlayerController currentPlayer;
        Transform activeProjectile;

        const float Zerof = 0f;

        void OnEnable()
        {
            CannonFiringHandler.OnFireCannon += ExitCannonAfterDelay;
            CannonFiringHandler.OnDryFireCannon += ExitCannonMode;
        }

        void OnDisable()
        {
            CannonFiringHandler.OnFireCannon -= ExitCannonAfterDelay;
            CannonFiringHandler.OnDryFireCannon -= ExitCannonMode;
        }

        void Awake()
        {
            currentPlayer = null;
        }

        public void PlayerFiringCannon()
        {
            firingHandler.FireCannon();
        }

        public void Interact(Transform interactor)
        {
            if (interactor.TryGetComponent(out PlayerController player))
            {
                if (currentPlayer == player) ExitCannonMode();
                else
                {
                    currentPlayer = player;
                    player.PlayerCannonMode(true, this);
                }
            }
        }

        public void EnterCannonMode(InputReader input)
        {
            if (currentPlayer != null)
            {
                currentPlayer.transform.SetPositionAndRotation(
                    transform.position + positionOffset,
                    transform.rotation);

                aimingHandler.SetInputReader(input); 
                CameraManager.Instance.SetTarget(cannonCameraTarget);
            }
        }

        void ExitCannonMode()
        {
            // Cannon may or may not have been fired. Check if already reset first
            if (aimingHandler != null) aimingHandler.SetInputReader(null);

            currentPlayer.PlayerCannonMode(false, null);
            currentPlayer = null;
            CameraManager.Instance.ReturnToDefault();

            aimingHandler.ResetCannonPosition();
            StartCoroutine(aimingHandler.ResetCannonRotationAfterDelay(1f));
        }

        void ExitCannonAfterDelay(float _)
        {
            StartCoroutine(ExitCannonModeAfterDelay(1f));
        }

        IEnumerator ExitCannonModeAfterDelay(float delay)
        {
            // Cannon was fired, so lock cannon after firing by clearing input
            aimingHandler.SetInputReader(null);
            yield return new WaitForSeconds(delay);
            CameraManager.Instance.ReturnToDefault();
            ExitCannonMode();
        }
    }
}
