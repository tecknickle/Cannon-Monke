using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class ShootableObjectHandler : MonoBehaviour, IShootable
    {
        [Header("Kinematic Duration")]
        [SerializeField] float ignoreCollisionDuration = 1f;

        Transform currentCannonLoadPosition;
        Rigidbody rb;
        Collider objectCollider;

        CountdownTimer ignoreCollisionTimer;

        void Update()
        {
            ignoreCollisionTimer.Tick(Time.deltaTime);

            if (currentCannonLoadPosition != null)
            {
                currentCannonLoadPosition.GetPositionAndRotation(
                    out Vector3 position,
                    out Quaternion rotation);

                transform.SetPositionAndRotation(position, rotation);
            }
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            objectCollider = GetComponent<Collider>();

            ignoreCollisionTimer = new CountdownTimer(ignoreCollisionDuration);

            ignoreCollisionTimer.OnTimerStart += () => objectCollider.enabled = false;
            ignoreCollisionTimer.OnTimerStop += () => objectCollider.enabled = true;
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("CannonLoadingZone"))
            {
                Debug.Log("Object hit LZ");

                CannonLoadingHandler cannon = 
                    other.collider.GetComponentInParent<CannonLoadingHandler>();

                if (cannon.IsCannonLoaded)
                {
                    Debug.Log("Cannon already loaded");
                    return;
                }
                else
                {
                    Debug.Log("Sending Cannon this object to load");
                    cannon.LoadTheCannon(this);
                }
            }
        }

        void OnCollisionExit(Collision other)
        {
            // no op
        }

        public void GetIntoCannon(Transform designatedLoadedPosition)
        {
            Debug.Log("Cannon is letting object in");
            currentCannonLoadPosition = designatedLoadedPosition;

            transform.SetPositionAndRotation(
                designatedLoadedPosition.position, 
                designatedLoadedPosition.rotation);

            rb.isKinematic = true; // Prevent physics while in cannon
        }

        public void GetLaunchedFromCannon(Vector3 direction, float force)
        {
            currentCannonLoadPosition = null;
            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
