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
            Shootable.OnHitCannonLoadingZone += DoLoadingVFX;
        }

        void OnDisable()
        {
            Shootable.OnHitCannonLoadingZone -= DoLoadingVFX;
        }

        private void DoLoadingVFX(Shootable _, Transform ojectTransform)
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
