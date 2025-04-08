using KBCore.Refs;
using System.Collections;
using Unity.Cinemachine;
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

        [Header("SO Events")]
        [SerializeField, Anywhere] FloatEventSO onCannonFiredChannel;
        [SerializeField, Anywhere] VoidEventSO onCannonDryFire;

        [Header("Settings")]
        [SerializeField] Vector3 positionOffset = new(0f, -2f, -6f);
        [SerializeField] float exitDelay = 1f;
        [SerializeField] float cannonResetDelay = 1f;

        PlayerController currentPlayer;
        Coroutine exitCannonModeCoroutine = null;

        const float Zerof = 0f;

        void OnEnable()
        {
            onCannonFiredChannel.RegisterListener(ExitCannonAfterDelay);
            onCannonDryFire.RegisterListener(ExitCannonMode);
        }

        void OnDisable()
        {
            onCannonFiredChannel.UnregisterListener(ExitCannonAfterDelay);
            onCannonDryFire.UnregisterListener(ExitCannonMode);
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
                if (currentPlayer == player)
                {
                    ExitCannonMode();
                    CameraManager.Instance.ReturnToDefault();
                }
                else
                {
                    currentPlayer = player;
                    player.PlayerCannonMode(true, this);
                }
            }
        }

        public void EnterCannonMode(InputReader input)
        {
            if (exitCannonModeCoroutine != null)
            {
                StopCoroutine(exitCannonModeCoroutine);
                exitCannonModeCoroutine = null;
            }
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
            aimingHandler.ResetCannonAimedAxis();
            exitCannonModeCoroutine = 
                StartCoroutine(
                    aimingHandler.ResetCannonRotationAfterDelay(cannonResetDelay)
                    );
        }

        void ExitCannonAfterDelay(float _)
        {
            StartCoroutine(ExitCannonModeAfterDelay(exitDelay));
        }

        IEnumerator ExitCannonModeAfterDelay(float delay)
        {
            // Cannon was fired, so lock cannon after firing by clearing input
            
            yield return new WaitForSeconds(delay);
            aimingHandler.SetInputReader(null);
            ExitCannonMode();
        }
    }
}
