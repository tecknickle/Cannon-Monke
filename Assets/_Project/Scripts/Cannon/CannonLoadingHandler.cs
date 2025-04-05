using KBCore.Refs;
using UnityEngine;

namespace CannonMonke
{
    public class CannonLoadingHandler : MonoBehaviour 
    {
        [Header("Loaded Position")]
        [SerializeField, Anywhere] Transform loadedPosition;

        IShootable objectToFire;
        public bool IsCannonLoaded;

        void Awake()
        {
            IsCannonLoaded = false;
        }

        public void LoadTheCannon(IShootable objectToLoad)
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
            }
        }

        public void FireTheObject(float force)
        {
            if (objectToFire != null)
            {
                objectToFire.GetLaunchedFromCannon(loadedPosition.transform.up, force);
                objectToFire = null;
            }
            IsCannonLoaded = false;
        }
    }
}
