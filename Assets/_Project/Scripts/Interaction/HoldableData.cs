using UnityEngine;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "HoldableData", menuName = "CannonMonke/HoldableData")]
    public class HoldableData : EntityData
    {
        // Additional properties specific to holdable objects
        public float mass;
        public AudioClip softImpactSound;
        public AudioClip hardImpactSound;
        public ParticleSystem defaultImpactVFX;
        public GameObject prefabVFX;
        public float launchVelocityMultiplier = 1f;
    }
}
