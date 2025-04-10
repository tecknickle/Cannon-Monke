using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class CrateGenericOrdinanceVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject crateHitPrefab;
        [SerializeField] CinemachineImpulseSource impulseSource;

        [Header("VFX Settings")]
        [SerializeField] float vfxScaleFactor = 5f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            CrateGenericOrdinance.OnCrateHit += HandleCrateHit;
        }

        void OnDisable()
        {
            CrateGenericOrdinance.OnCrateHit -= HandleCrateHit;
        }

        void HandleCrateHit(Transform location)
        {
            if (crateHitPrefab != null)
            {
                GameObject vfxInstance1 = Instantiate(
                    crateHitPrefab,
                    location.transform.position,
                    Quaternion.identity);

                vfxInstance1.transform.localScale *= vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance1, vfxLifetime); // Clean up after 2 seconds
            }
            impulseSource.GenerateImpulse();
            // placeholder for future sound
            SoundManager.PlaySound(SoundType.CrateGenericOrdinance, 0.5f);
        }
    }
}

