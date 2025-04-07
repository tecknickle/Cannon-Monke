using KBCore.Refs;
using System.Collections;
using UnityEngine;

namespace CannonMonke
{
    public class CannonAimingHandler : MonoBehaviour
    {
        [SerializeField, Anywhere] Transform cannonBarrel;
        [SerializeField, Anywhere] Transform cannonBase;

        [Header("Aiming Settings")]
        [SerializeField] float xAxisRotationSpeed = 100f;
        [SerializeField] float yAxisRotationSpeed = 100f;
        [SerializeField] float interpolationSpeed = 20f;

        [Header("Clamping Settings")]
        [SerializeField] float upperLimitInDegrees = 1f;
        [SerializeField] float lowerLimitInDegrees = 60f;
        [SerializeField] float leftLimitInDegrees = -45f;
        [SerializeField] float rightLimitInDegrees = 45f;

        float yAxisRotation;
        float xAxisRotation;

        InputReader currentInputReader;

        const float Zerof = 0f;

        void Awake()
        {
            currentInputReader = null;
        }

        void Update()
        {
            if (currentInputReader != null)
            {
                HandleAimingRotation();
            }
        }

        public void SetInputReader(InputReader inputReader)
        {
            if (inputReader == null)
            {
                currentInputReader = null;
            }
            currentInputReader = inputReader;
        }

        void HandleAimingRotation()
        {
            float xAxisRotationInput =
                currentInputReader.Direction.y
                * xAxisRotationSpeed * Time.deltaTime;

            float yAxisRotationInput =
                currentInputReader.Direction.x
                * yAxisRotationSpeed * Time.deltaTime;

            yAxisRotation += yAxisRotationInput;
            yAxisRotation = Mathf.Clamp(
                yAxisRotation,
                leftLimitInDegrees,
                rightLimitInDegrees); // Clamp horizontal rotation

            xAxisRotation += xAxisRotationInput;
            xAxisRotation = Mathf.Clamp(
                xAxisRotation,
                upperLimitInDegrees,
                lowerLimitInDegrees); // Clamp vertical rotation

            cannonBarrel.transform.localRotation = Quaternion.Slerp(
                cannonBarrel.transform.localRotation,
                Quaternion.Euler(xAxisRotation, yAxisRotation, Zerof),
                interpolationSpeed * Time.deltaTime);

            cannonBase.transform.localRotation = Quaternion.Slerp(
                cannonBase.transform.localRotation,
                Quaternion.Euler(Zerof, yAxisRotation, Zerof), // Only rotate the base horizontally
                interpolationSpeed * Time.deltaTime);
        }

        public IEnumerator ResetCannonRotationAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Quaternion barrelStartRotation = cannonBarrel.transform.localRotation;
            Quaternion baseStartRotation = cannonBase.transform.localRotation;

            float elapsedTime = 0f;
            SoundManager.PlaySound(SoundType.CannonReset, 1f);

            while (elapsedTime < 1f)
            {
                cannonBarrel.transform.localRotation = Quaternion.Slerp(
                    barrelStartRotation,
                    Quaternion.identity, // Reset to default rotation
                    elapsedTime);

                cannonBase.transform.localRotation = Quaternion.Slerp(
                    baseStartRotation,
                    Quaternion.identity, // Reset to default rotation
                    elapsedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            cannonBarrel.transform.localRotation = Quaternion.identity;
            cannonBase.transform.localRotation = Quaternion.identity;
            // Ensure final rotation is set to identity
        }

        public void ResetCannonPosition()
        {
            xAxisRotation = Zerof; // Reset x-axis rotation
            yAxisRotation = Zerof; // Reset y-axis rotation
        }
    }
}
