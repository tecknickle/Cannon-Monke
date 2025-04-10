using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    // Lives on Explosive Objects
    public class ExplosiveOrdinanceVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] GameObject firingPrefab;
        [SerializeField] CinemachineImpulseSource impulseSource;

        [Header("VFX Settings")]
        [SerializeField] float vfxScaleFactor = 5f;
        [SerializeField] float vfxLifetime = 2f;

        void OnEnable()
        {
            ExplosiveOrdinance.OnExploded += HandleExplosion;
        }

        void OnDisable()
        {
            ExplosiveOrdinance.OnExploded -= HandleExplosion;
        }

        void HandleExplosion(Transform location)
        {
            if (firingPrefab != null)
            {
                GameObject vfxInstance1 = Instantiate(
                    firingPrefab,
                    location.transform.position,
                    Quaternion.identity);

                vfxInstance1.transform.localScale *= vfxScaleFactor; // Scaling based on force
                Destroy(vfxInstance1, vfxLifetime); // Clean up after 2 seconds
            }
            impulseSource.GenerateImpulse();
            // placeholder for future sound
            SoundManager.PlaySound(SoundType.ExplosiveOrdinance, 0.5f);
        }
    }
}

