using KBCore.Refs;
using System.Collections;
using System.Threading;
using System.Transactions;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class CannonController : ValidatedMonoBehaviour, IInteractable
    {
        [Header("References")]
        [SerializeField, Self] CannonLoadingHandler loadingHandler;
        [SerializeField, Anywhere] Transform cannonBarrel;
        [SerializeField, Anywhere] Transform cannonBase;
        [SerializeField, Anywhere] Transform cannonCameraTarget;
        [SerializeField, Self] CinemachineImpulseSource impulseSource;

        [Header("Cannon Firing Settings")]
        [SerializeField] float cannonFiringForce = 20f;

        [Header("Aiming Settings")]
        [SerializeField] float xAxisRotationSpeed = 100f;
        [SerializeField] float yAxisRotationSpeed = 100f;
        [SerializeField] float interpolationSpeed = 20f;

        [Header("Clamping Settings")]
        [SerializeField] float upperLimitInDegrees = 1f;
        [SerializeField] float lowerLimitInDegrees = 60f;
        [SerializeField] float leftLimitInDegrees = -45f;
        [SerializeField] float rightLimitInDegrees = 45f;

        [Header("Other Settings")]
        [SerializeField] Vector3 positionOffset = new(0f, -2f, -6f);

        PlayerController currentPlayer;
        InputReader currentInputReader;
        Transform activeProjectile;

        const float Zerof = 0f;

        float yAxisRotation;
        float xAxisRotation;

        void Awake()
        {
            currentPlayer = null;
            currentInputReader = null;
        }

        void Update()
        {
            if (currentPlayer != null)
            {
                HandleAimingRotation();
            }
        }

        public void HandleAimingRotation()
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

        public void FireCannon()
        {
            Debug.Log("Trying to Fire Cannon...");
            if (loadingHandler.IsCannonLoaded)
            {
                activeProjectile = loadingHandler.loadedObjectTransform;
                loadingHandler.FireTheObject(cannonFiringForce);

                Debug.Log("Firing cannon!");
                
                SoundManager.PlaySound(SoundType.CannonFire, 1f);

                impulseSource.GenerateImpulse();
                // TEMPORARY: will be changed to toggle between active and player
                // ******************************************
                //CameraManager.Instance.SetTarget(activeProjectile);

                StartCoroutine(ExitCannonModeAfterDelay(1f));
            }
            else
            {
                CameraManager.Instance.ReturnToDefault();
                Debug.Log("Cannon is not loaded, cannot fire.");
                SoundManager.PlaySound(SoundType.CannonDryFire, 1f);
                ExitCannonMode();
            }
        }

        public void EnterCannonMode(InputReader input)
        {
            if (currentPlayer != null)
            {
                currentPlayer.transform.SetPositionAndRotation(
                    transform.position + positionOffset,
                    transform.rotation);

                currentInputReader = input;
                CameraManager.Instance.SetTarget(cannonCameraTarget);
            }
        }

        public void ExitCannonMode()
        {
            currentInputReader = null;

            currentPlayer.PlayerCannonMode(false, null);
            currentPlayer = null;

            // reset cannon position/orientation to parents'
            xAxisRotation = Zerof;
            yAxisRotation = Zerof;

            StartCoroutine(ResetCannonRotationAfterDelay(1f));
        }

        IEnumerator ExitCannonModeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            //TEMPORARY: will be changed to toggle between active and player
            //*********************************************************
            CameraManager.Instance.ReturnToDefault();
            ExitCannonMode();
        }

        IEnumerator ResetCannonRotationAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            SoundManager.PlaySound(SoundType.CannonReset, 1f);

            Quaternion barrelStartRotation = cannonBarrel.transform.localRotation;
            Quaternion baseStartRotation = cannonBase.transform.localRotation;

            float elapsedTime = 0f;

            while(elapsedTime < 1f)
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
            cannonBase.transform.localRotation = Quaternion.identity; // Ensure final rotation is set to identity
        }

        public void Interact(Transform interactor)
        {
            if (interactor.TryGetComponent(out PlayerController player))
            {
                // If player is currently in cannon mode,
                // will exit cannon on second interaction
                if (currentPlayer == player)
                {
                    ExitCannonMode();
                }
                else
                {
                    currentPlayer = player;
                    player.PlayerCannonMode(true, this);
                }
            }
        }
    }
}
