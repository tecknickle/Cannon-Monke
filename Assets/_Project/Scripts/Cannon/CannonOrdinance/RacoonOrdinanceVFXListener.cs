using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class RacoonOrdinanceVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject racoonHitPrefab;
        [SerializeField] CinemachineImpulseSource impulseSource;

        [Header("VFX Settings")]
        [SerializeField] float vfxScaleFactor = 5f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            RacoonOrdinance.OnRacoonHit += HandleRacoonHit;
        }

        void OnDisable()
        {
            RacoonOrdinance.OnRacoonHit -= HandleRacoonHit;
        }

        void HandleRacoonHit(Transform location)
        {
            if (racoonHitPrefab != null)
            {
                GameObject vfxInstance1 = Instantiate(
                    racoonHitPrefab,
                    location.transform.position,
                    Quaternion.identity);

                vfxInstance1.transform.localScale *= vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance1, vfxLifetime); // Clean up after 2 seconds
            }
            impulseSource.GenerateImpulse();
            // placeholder for future sound
            SoundManager.PlaySound(SoundType.RacoonOrdinance, 0.5f);
        }
    }
}

