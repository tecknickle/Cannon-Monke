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

        void Awake()
        {
            IsCannonLoaded = false;
            objectToFire = null;
            loadedObjectTransform = null;
        }

        public void LoadTheCannon(IShootable objectToLoad, Transform objectTransform)
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
