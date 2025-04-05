using DG.Tweening;
using System.Collections;
using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class ShootableObjectHandler : MonoBehaviour, IShootable
    {
        [Header("Shootable Settings")]
        [SerializeField] float ignoreCollisionDuration = 1f;
        // This offset is used to move the object into the cannon when it is loaded
        [SerializeField] Vector3 coroutineStartOffset = new(0f, 5f, 0f);


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
            // no op for now
        }

        public void GetIntoCannon(Transform designatedLoadedPosition)
        {
            Debug.Log("Cannon is letting object in: " + this.name);
            currentCannonLoadPosition = designatedLoadedPosition;

            StartCoroutine(MoveObjectIntoCannon(0.5f));

            rb.isKinematic = true; // Prevent physics while in cannon
        }

        IEnumerator MoveObjectIntoCannon(float time)
        {
            Vector3 startPosition = currentCannonLoadPosition.position
                + coroutineStartOffset;

            Quaternion startRotation = transform.rotation;

            float elapsedTime = 0f;

            while(elapsedTime < time)
            {
                transform.SetPositionAndRotation(
                    (Vector3.Lerp(
                    startPosition,
                    currentCannonLoadPosition.position,
                    elapsedTime)),
                    (Quaternion.Lerp(
                    startRotation,
                    currentCannonLoadPosition.rotation,
                    elapsedTime)));

                elapsedTime += Time.deltaTime;
                yield return null;
            }
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
