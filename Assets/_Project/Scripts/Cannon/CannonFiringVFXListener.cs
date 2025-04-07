using System;
using UnityEngine;

namespace CannonMonke
{
    public class CannonFiringVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform effectPosition;
        [SerializeField] FloatEventSO onCannonFiredChannel;
        [SerializeField] GameObject firingPrefab;
        [SerializeField] GameObject firingTextPrefab;

        [Header("VFX Settings")]
        [Tooltip("This is used to scale the VFX based on the cannon force. " +
            "Set this to a value that matches your VFX prefab's scale. " +
            "VFX Scale = force / scaleFactor")]
        [SerializeField] float vfxScaleFactor = 1f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            onCannonFiredChannel.RegisterListener(DoFiringVFX);
        }

        void OnDisable()
        {
            onCannonFiredChannel.UnregisterListener(DoFiringVFX);
        }

        void DoFiringVFX(float force)
        {
            if (firingPrefab != null)
            {
                GameObject vfxInstance1 = Instantiate(
                    firingPrefab, 
                    effectPosition.transform.position, 
                    Quaternion.identity);

                vfxInstance1.transform.localScale *= force / vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance1, vfxLifetime); // Clean up after 2 seconds
            }

            if (firingTextPrefab != null)
            {
                GameObject vfxInstance2 = Instantiate(
                    firingTextPrefab,
                    effectPosition.transform.position,
                    Quaternion.identity);

                vfxInstance2.transform.localScale *= force / vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance2, vfxLifetime); // Clean up after 2 seconds
            }
        }
    }
}
