using UnityEngine;

namespace CannonMonke
{
    public class CannonFireVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform effectPosition;
        [SerializeField] FloatEventSO onCannonFired;
        [SerializeField] GameObject vfxPrefab;

        [Header("VFX Settings")]
        [Tooltip("This is used to scale the VFX based on the cannon force. " +
            "Set this to a value that matches your VFX prefab's scale. " +
            "VFX Scale = force / scaleFactor")]
        [SerializeField] float vfxScaleFactor = 1f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable() => onCannonFired.RegisterListener(DoCannonVFX);
        void OnDisable() => onCannonFired.UnregisterListener(DoCannonVFX);

        void DoCannonVFX(float force)
        {
            if (vfxPrefab != null)
            {
                GameObject vfxInstance = Instantiate(
                    vfxPrefab, 
                    effectPosition.transform.position, 
                    Quaternion.identity);

                vfxInstance.transform.localScale *= force / vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance, vfxLifetime); // Clean up after 2 seconds
            }
        }
    }
}
