using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class CannonSmokingBarrelVFXListener : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Transform effectPosition;
        [SerializeField] GameObject smokingPrefab;

        [Header("VFX Settings")]
        [SerializeField] float vfxScale = 2f;
        [SerializeField] float vfxDuration = 2f;
        [SerializeField] Vector3 offsetFromPosition = new(0f, -3f, 0f);

        void OnEnable()
        {
            CannonAimingHandler.OnCannonResetting += DoSmokingVFX;
        }

        void OnDisable()
        {
            CannonAimingHandler.OnCannonResetting -= DoSmokingVFX;
        }

        void DoSmokingVFX()
        {
            StartCoroutine(SmokeFollowCannon());
        }

        IEnumerator SmokeFollowCannon()
        {
            if (smokingPrefab != null)
            {
                smokingPrefab.SetActive(true);
                float timeElapsed = 0f;

                Quaternion upRotation = Quaternion.Euler(-90f, 0f, 0f);

                GameObject vfxInstance = Instantiate(
                    smokingPrefab,
                    effectPosition.position + offsetFromPosition,
                    upRotation);

                vfxInstance.transform.localScale *= vfxScale;

                while (timeElapsed < vfxDuration)
                {
                    vfxInstance.transform.position = 
                        effectPosition.transform.position 
                        + offsetFromPosition;

                    vfxInstance.transform.rotation =
                        effectPosition.transform.rotation * upRotation;

                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                vfxInstance.transform.localRotation = upRotation;
                smokingPrefab.SetActive(false);
                Destroy(vfxInstance, vfxDuration);
            }
        }
    }
}
