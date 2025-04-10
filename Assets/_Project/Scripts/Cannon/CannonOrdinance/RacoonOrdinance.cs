using System;
using UnityEngine;

namespace CannonMonke
{
    public class RacoonOrdinance : MonoBehaviour, IOrdinance
    {
        [Header("Splatter Settings")]
        [SerializeField] float racoonHitForce = 5f;
        [SerializeField] float racoonHitRadius = 10f;
        [SerializeField] float racoonHitUpwardsModifier = 5f;

        public static event Action<Transform> OnRacoonHit;

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
            OnRacoonHit?.Invoke(this.transform);
            Collider[] colliders = Physics.OverlapSphere(
                transform.position, racoonHitRadius);

            foreach (var collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(
                        racoonHitForce,
                        transform.position,
                        racoonHitRadius,
                        racoonHitUpwardsModifier,
                        ForceMode.Impulse);
                }
            }
        }
    }
}

