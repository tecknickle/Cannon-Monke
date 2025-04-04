using KBCore.Refs;
using UnityEngine;

namespace CannonMonke
{
    public class PlayerObjectHolding : MonoBehaviour 
    {
        [Header("Holding Position")]
        [SerializeField, Anywhere] Transform holdingPosition;

        const float DefaultDropForce = 1f;

        IHoldable heldObject;
        public bool isHolding;

        void Awake()
        {
            isHolding = false;
        }

        public void PickUpObject(IHoldable objectToHold)
        {
            // If not holding anything, pick up object
            if (heldObject != null)
            {
                // Reset current held object in order to pick up next object
                DropObject(DefaultDropForce);
                heldObject = null;

                heldObject = objectToHold;
                heldObject.Pickup(holdingPosition.transform);
            }
            else
            {
                heldObject = objectToHold;
                heldObject.Pickup(holdingPosition.transform);
            }
            isHolding = true;
        }

        // Used methods in PlayerController
        public void DropObject(float dropForce)
        {
            if (heldObject != null)
            {
                // A weaker throw so object doesn't sit on player after drop
                heldObject.Throw(holdingPosition.transform.forward, dropForce);
                heldObject = null;
            }
            isHolding = false;
        }

        public void ThrowObject(float throwForce)
        {
            if (heldObject != null)
            {
                heldObject.Throw(holdingPosition.transform.up + transform.forward, throwForce);
                heldObject = null;
            }
            isHolding = false;
        }
    }
}
