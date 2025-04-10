using System;
using UnityEngine;

namespace CannonMonke
{
    public class CrateGenericOrdinance : MonoBehaviour, IOrdinance
    {
        [Header("Splatter Settings")]
        [SerializeField] float crateHitForce = 5f;
        [SerializeField] float crateHitRadius = 10f;
        [SerializeField] float crateHitUpwardsModifier = 5f;

        public static event Action<Transform> OnCrateHit;

        void Awake()
        {
            enabled = false;
        }

        public void EnableOrdinance()
        {
            enabled = true;
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (enabled == false) return;
            OnHit();
            gameObject.SetActive(false);
        }

        public void OnHit()
        {
            OnCrateHit?.Invoke(this.transform);
            Collider[] colliders = Physics.OverlapSphere(
                transform.position, crateHitRadius);

            foreach (var collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(
                        crateHitForce,
                        transform.position,
                        crateHitRadius,
                        crateHitUpwardsModifier,
                        ForceMode.Impulse);
                }
            }
        }
    }
}

