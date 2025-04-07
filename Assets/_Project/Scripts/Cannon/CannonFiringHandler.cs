using KBCore.Refs;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class CannonFiringHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] CannonLoadingHandler loadingHandler;
        [SerializeField, Self] CinemachineImpulseSource impulseSource;
        [SerializeField, Anywhere] Transform cannonCameraTarget;

        [Header("SO Events")]
        [SerializeField, Anywhere] FloatEventSO onCannonFiredChannel;
        [SerializeField, Anywhere] VoidEventSO onCannonDryFire;

        [Header("Cannon Firing Settings")]
        [SerializeField] float cannonFiringForce = 30f;

        Transform activeProjectile;

        const float Zerof = 0f;

        void OnEnable()
        {
            PlayerController.OnPlayerCameraSwitch += SwitchCamera;
        }

        void OnDisable()
        {
            PlayerController.OnPlayerCameraSwitch -= SwitchCamera;
        }

        private void Awake()
        {
            activeProjectile = null;
        }

        public void FireCannon()
        {
            if (loadingHandler.IsCannonLoaded)
            {
                onCannonFiredChannel.Raise(cannonFiringForce);
                loadingHandler.FireTheObject(cannonFiringForce);
                CameraManager.Instance.SetTarget(activeProjectile);
                impulseSource.GenerateImpulse();
                SoundManager.PlaySound(SoundType.CannonFire, 1f);
            }
            else
            {
                onCannonDryFire.Raise();
                CameraManager.Instance.ReturnToDefault();
                SoundManager.PlaySound(SoundType.CannonDryFire, 1f);
                Debug.Log("Cannon is not loaded, cannot fire.");
            }
        }

        public void SetActiveProjectile(Transform projectile)
        {
            activeProjectile = projectile;
        }

        void SwitchCamera(bool isInCannonMode)
        {
            if (activeProjectile != null && isInCannonMode)
            {
                CameraManager.Instance.ToggleBetweenCannonAndProjectile(
                    cannonCameraTarget, // Reference must be set in editor for now
                    activeProjectile);
            }
            else if (activeProjectile != null && !isInCannonMode)
            {
                CameraManager.Instance.ToggleBetweenDefaultAnd(activeProjectile);
            }
            else
            {
                Debug.Log("No active projectile yet.");
                return;
            }
        }
    }
}
