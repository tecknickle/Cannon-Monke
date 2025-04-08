using KBCore.Refs;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class ExplosiveComponent : MonoBehaviour
    {
        [Header("References")]

        [Header("Explosion Settings")]
        [SerializeField] float explosionForce = 20f;
        [SerializeField] float explosionRadius = 50f;
        [SerializeField] float explosionUpwardsModifier = 20f;

        bool hasExploded;

        public static event Action<Transform> OnExploded;

        void Awake()
        {
            hasExploded = false;
        }

        public void EnableExplosive()
        {
            hasExploded = false;
            enabled = true;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (hasExploded) return;
            hasExploded = true;
            Explode();
            gameObject.SetActive(false);
        }

        void Explode()
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

