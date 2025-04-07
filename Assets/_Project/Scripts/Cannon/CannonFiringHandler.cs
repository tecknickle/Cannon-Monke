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

        private void Awake()
        {
            activeProjectile = null;
        }

        public void FireCannon()
        {
            if (loadingHandler.IsCannonLoaded)
            {
                OnFireCannon?.Invoke(cannonFiringForce);
                activeProjectile = loadingHandler.loadedObjectTransform;
                loadingHandler.FireTheObject(cannonFiringForce);

                impulseSource.GenerateImpulse();
                SoundManager.PlaySound(SoundType.CannonFire, 1f);
            }
            else
            {
                OnDryFireCannon?.Invoke();
                Debug.Log("Cannon is not loaded, cannot fire.");
                SoundManager.PlaySound(SoundType.CannonDryFire, 1f);
            }
        }
    }
}
