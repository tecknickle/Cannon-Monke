using KBCore.Refs;
using UnityEngine;

namespace CannonMonke
{
    public class CannonLoadingHandler : MonoBehaviour 
    {
        [Header("Loaded Position")]
        [SerializeField, Anywhere] Transform loadedPosition;

        IShootable objectToFire;
        public Transform loadedObjectTransform;
        public bool IsCannonLoaded;

        void OnEnable()
        {
            Shootable.OnHitCannonLoadingZone += HandleOnHitCannonLoadingZone;
        }
        void OnDisable()
        {
            Shootable.OnHitCannonLoadingZone -= HandleOnHitCannonLoadingZone;
        }

        void Awake()
        {
            IsCannonLoaded = false;
            objectToFire = null;
            loadedObjectTransform = null;
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

                SoundManager.PlaySound(SoundType.CannonLoad, 1f);
            }
        }

        public void FireTheObject(float force)
        {
            if (objectToFire != null)
            {
                objectToFire.GetLaunchedFromCannon(loadedPosition.transform.up, force);
                objectToFire = null;
                loadedObjectTransform = null;
            }
            IsCannonLoaded = false;
        }
    }
}
