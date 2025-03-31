using UnityEngine;

namespace CannonMonke
{
    public interface IHoldable 
    {
        void Pickup(Transform holder);
        void Drop();
        void Throw(Vector3 direction, float force);
    }
}
