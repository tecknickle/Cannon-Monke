using System;
using UnityEngine;

namespace CannonMonke
{
    public class SplatterOrdinance : MonoBehaviour, IOrdinance
    {
        [Header("Splatter Settings")]
        [SerializeField] float splatterForce = 5f;
        [SerializeField] float splatterRadius = 10f;
        [SerializeField] float splatterUpwardsModifier = 5f;

        public static event Action<Transform> OnSplatter;

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
            OnSplatter?.Invoke(this.transform);
            Collider[] colliders = Physics.OverlapSphere(
                transform.position, splatterRadius);

            foreach (var collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(
                        splatterForce,
                        transform.position,
                        splatterRadius,
                        splatterUpwardsModifier,
                        ForceMode.Impulse);
                }
            }
        }
    }
}

