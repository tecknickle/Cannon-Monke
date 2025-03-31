using UnityEngine;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "HoldableObjectData", menuName = "CannonMonke/Holdable Object Data")]

    public class HoldableObjectData : EntityData
    {
        public float objectWeight;
        // Additional properties specific to holdable objects
    }
}
