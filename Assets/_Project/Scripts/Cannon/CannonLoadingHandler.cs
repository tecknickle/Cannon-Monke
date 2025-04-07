using KBCore.Refs;
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

        IShootable objectToFire;
        Transform loadedObjectTransform;
        Transform lastFiredObject;
        public bool IsCannonLoaded;

        void OnEnable()
        {
            Shootable.OnHitCannonLoadingZone += HandleOnHitCannonLoadingZone;
            onCannonFiredChannel.RegisterListener(FireTheObject);
        }
        void OnDisable()
        {
            Shootable.OnHitCannonLoadingZone -= HandleOnHitCannonLoadingZone;
            onCannonFiredChannel.UnregisterListener(FireTheObject);
        }

        void Awake()
        {
            IsCannonLoaded = false;
            objectToFire = null;
            loadedObjectTransform = null;
            lastFiredObject = null;
        }

        void HandleOnHitCannonLoadingZone(Shootable newShootable, Transform newObject)
        {
            if (newShootable == null) return;
            if (!IsCannonLoaded) LoadTheCannon(newShootable, newObject);
            else
            {
                Debug.Log("Cannon already loaded, ignoring additional hit.");
            }
        }

        void LoadTheCannon(IShootable objectToLoad, Transform objectTransform)
        {
            if (objectToFire != null)
            {
                Debug.Log("Cannon is already loaded.");
                return;
            }
            else
            {
                Debug.Log("Trying to load cannon");
                objectToFire = objectToLoad;
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
