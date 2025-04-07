using KBCore.Refs;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class CannonFiringHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] CannonLoadingHandler loadingHandler;
        [SerializeField, Self] CinemachineImpulseSource impulseSource;

        [Header("Cannon Firing Settings")]
        [SerializeField] float cannonFiringForce = 30f;

        Transform activeProjectile;

        const float Zerof = 0f;

        public static event Action<float> OnFireCannon;
        public static event Action OnDryFireCannon;

        void OnEnable()
        {
            PlayerController.OnPlayerSwitchCameraRequested += SwitchCamera;// Subscribe to events if needed
        }

        void OnDisable()
        {
            PlayerController.OnPlayerSwitchCameraRequested -= SwitchCamera; // Unsubscribe from events
        }

        private void Awake()
        {
            activeProjectile = null;
        }

        public void FireCannon()
        {
            if (loadingHandler.IsCannonLoaded)
            {
                OnFireCannon?.Invoke(cannonFiringForce);
                loadingHandler.FireTheObject(cannonFiringForce);
                CameraManager.Instance.SetTarget(activeProjectile);
                impulseSource.GenerateImpulse();
                SoundManager.PlaySound(SoundType.CannonFire, 1f);
            }
            else
            {
                OnDryFireCannon?.Invoke();
                CameraManager.Instance.ReturnToDefault();
                SoundManager.PlaySound(SoundType.CannonDryFire, 1f);
                Debug.Log("Cannon is not loaded, cannot fire.");
            }
        }

        public void SetActiveProjectile(Transform projectile)
        {
            activeProjectile = projectile;
        }

        void SwitchCamera()
        {
            if (activeProjectile != null)
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
