using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class SplatterOrdinanceVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject splatterPrefab;
        [SerializeField] CinemachineImpulseSource impulseSource;

        [Header("VFX Settings")]
        [SerializeField] float vfxScaleFactor = 5f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            SplatterOrdinance.OnSplatter += HandleSplatter;
        }

        void OnDisable()
        {
            SplatterOrdinance.OnSplatter -= HandleSplatter;
        }

        void HandleSplatter(Transform location)
        {
            if (splatterPrefab != null)
            {
                GameObject vfxInstance1 = Instantiate(
                    splatterPrefab,
                    location.transform.position,
                    Quaternion.identity);

                vfxInstance1.transform.localScale *= vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance1, vfxLifetime); // Clean up after 2 seconds
            }
            impulseSource.GenerateImpulse();
            // placeholder for future sound
            SoundManager.PlaySound(SoundType.SplatterOrdinance, 0.5f);
        }
    }
}

