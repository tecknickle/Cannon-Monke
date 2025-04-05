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
                Debug.Log("Object hit LZ: " + this.name);

                CannonLoadingHandler cannon = 
                    other.collider.GetComponentInParent<CannonLoadingHandler>();

                if (cannon.IsCannonLoaded)
                {
                    Debug.Log("Cannon already loaded. Reject: " + this.name);
                    return;
                }
                else
                {
                    Debug.Log("Sending Cannon to load" + this.name);
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
            Debug.Log("Cannon is letting object in: " + this.name);
            currentCannonLoadPosition = designatedLoadedPosition;

            transform.SetPositionAndRotation(
                designatedLoadedPosition.position, 
                designatedLoadedPosition.rotation);

            rb.isKinematic = true; // Prevent physics while in cannon
        }

        public void GetLaunchedFromCannon(Vector3 direction, float force)
        {
            currentCannonLoadPosition = null;
            ignoreCollisionTimer.Start();
            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.Impulse);
            Debug.Log("I am being launched: " + this.name);
        }
    }
}
