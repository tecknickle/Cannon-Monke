using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class Shootable : MonoBehaviour, IShootable
    {
        [Header("Shootable Settings")]
        [SerializeField] float ignoreCollisionDuration = 1f;
        [SerializeField] Vector3 coroutineStartOffset = new(0f, 5f, 0f);
        [SerializeField] float slideIntoCannonDuration = 0.2f;
        [SerializeField] float minRandomSpin = 1f;
        [SerializeField] float maxRandomSpin = 7f;

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

        public void GetIntoCannon(Transform designatedLoadedPosition)
        {
            Debug.Log("Cannon is letting object in: " + this.name);
            currentCannonLoadPosition = designatedLoadedPosition;

            StartCoroutine(MoveObjectIntoCannon(slideIntoCannonDuration));

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
            // Ensure final position and rotation are set
            transform.SetPositionAndRotation(
                currentCannonLoadPosition.position,
                currentCannonLoadPosition.rotation);
        }

        public void GetLaunchedFromCannon(Vector3 direction, float force)
        {
            currentCannonLoadPosition = null;
            ignoreCollisionTimer.Start();
            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.Impulse);
            AddRandomSpin();
            Debug.Log("I am being launched: " + this.name);
        }

        void AddRandomSpin()
        {
            float randomNumber = UnityEngine.Random.Range(minRandomSpin, maxRandomSpin);
            rb.AddTorque(Vector3.right * randomNumber, ForceMode.Impulse);
        }
    }
}
