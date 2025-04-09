using System;
using UnityEngine;

namespace CannonMonke
{
    public class CannonLoadingVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform effectPosition;
        [SerializeField] GameObject loadingPrefab;

        [Header("VFX Settings")]
        [SerializeField] float vfxScale = 2f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            CannonLoadingZone.OnHitCannonLoadingZone += DoLoadingVFX;
        }

        void OnDisable()
        {
            CannonLoadingZone.OnHitCannonLoadingZone -= DoLoadingVFX;
        }

        private void DoLoadingVFX(Transform ojectTransform)
        {
            if (loadingPrefab != null)
            {
                GameObject vfxInstance = Instantiate(
                    loadingPrefab,
                    effectPosition.transform.position,
                    Quaternion.identity);

                vfxInstance.transform.localScale *= vfxScale;
                Destroy(vfxInstance, vfxLifetime);
            }
        }
    }
}
