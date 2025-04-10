using KBCore.Refs;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class ExplosiveOrdinance : MonoBehaviour, IOrdinance
    {
        [Header("Explosion Settings")]
        [SerializeField] float explosionForce = 20f;
        [SerializeField] float explosionRadius = 50f;
        [SerializeField] float explosionUpwardsModifier = 20f;

        public static event Action<Transform> OnExploded;

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
            OnExploded?.Invoke(this.transform);
            Collider[] colliders = Physics.OverlapSphere(
                transform.position, explosionRadius);

            foreach (var collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(
                        explosionForce,
                        transform.position,
                        explosionRadius,
                        explosionUpwardsModifier,
                        ForceMode.Impulse);
                }
            }
        }
    }
}

