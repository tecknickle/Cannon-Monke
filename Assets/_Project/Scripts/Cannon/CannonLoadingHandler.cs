using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class CannonLoadingHandler : MonoBehaviour 
    {
        [Header("Loaded Position")]
        [SerializeField, Self] CannonFiringHandler firingHandler;
        [SerializeField, Anywhere] Transform loadedPosition;

        [Header("SO Events")]
        [SerializeField, Anywhere] FloatEventSO onCannonFiredChannel;

        Shootable objectToFire;
        Transform loadedObjectTransform;
        Transform lastFiredObject;
        public bool IsCannonLoaded;

        void OnEnable()
        {
            CannonLoadingZone.OnHitCannonLoadingZone += HandleOnHitCannonLoadingZone;
            onCannonFiredChannel.RegisterListener(FireTheObject);
        }
        void OnDisable()
        {
            CannonLoadingZone.OnHitCannonLoadingZone -= HandleOnHitCannonLoadingZone;
            onCannonFiredChannel.UnregisterListener(FireTheObject);
        }

        void Awake()
        {
            IsCannonLoaded = false;
            objectToFire = null;
            loadedObjectTransform = null;
            lastFiredObject = null;
        }

        void HandleOnHitCannonLoadingZone(Transform newObject)
        {
            if (newObject == null || IsCannonLoaded) return;
            objectToFire = newObject.AddComponent<Shootable>();
            if (!IsCannonLoaded) LoadTheCannon(newObject);
            else
            {
                Debug.Log("Cannon already loaded, ignoring additional hit.");
            }
        }

        void LoadTheCannon(Transform objectTransform)
        {
            if (IsCannonLoaded)
            {
                Debug.Log("Cannon is already loaded.");
                return;
            }
            else
            {
                Debug.Log("Trying to load cannon");
                objectToFire.GetIntoCannon(loadedPosition);

                IsCannonLoaded = true;
                loadedObjectTransform = objectTransform; // Used for camera tracking

                SoundManager.PlaySound(SoundType.CannonLoad, 0.3f);
            }
        }

        public void FireTheObject(float force)
        {
            if (objectToFire != null)
            {
                lastFiredObject = loadedObjectTransform;
                objectToFire.GetLaunchedFromCannon(loadedPosition.transform.up, force);
                firingHandler.SetActiveProjectile(lastFiredObject);
                loadedObjectTransform = null;
                objectToFire = null;
                
            }
            IsCannonLoaded = false;
        }
    }
}

