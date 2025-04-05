using UnityEngine;

namespace CannonMonke
{
    public interface IShootable
    {
        void GetIntoCannon(Transform cannon);
        void GetLaunchedFromCannon(Vector3 direction, float force);
    }
}
