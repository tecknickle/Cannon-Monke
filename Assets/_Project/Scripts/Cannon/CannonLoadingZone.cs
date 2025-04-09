using KBCore.Refs;
using System;
using UnityEngine;

namespace CannonMonke
{
    [RequireComponent(typeof(Collider))]
    public class CannonLoadingZone : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere] Collider loadingZoneCollider;

        private void Awake()
        {
            loadingZoneCollider = GetComponent<Collider>();
        }
        // Transform is used for camera tracking active projectile in Cannon Controller
        public static event Action<Transform> OnHitCannonLoadingZone;

        void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (other.CompareTag("Holdable"))
            {
                Debug.Log("Object hit LZ: " + other.name);
                OnHitCannonLoadingZone?.Invoke(other.transform);
            }
        }
    }
}

