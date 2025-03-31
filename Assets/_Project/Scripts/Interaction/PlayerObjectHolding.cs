using KBCore.Refs;
using UnityEngine;

namespace CannonMonke
{
    public class PlayerObjectHolding : MonoBehaviour 
    {
        [Header("Holding Position")]
        [SerializeField, Anywhere] Transform holdingPosition;

        [Header("Object Handling Settings")]
        [SerializeField] float throwForce = 10f;
        [SerializeField] float dropForwardForce = 5f;

        IHoldable heldObject;

        public void PickUpObject(IHoldable objectToHold)
        {
            // If not holding anything, pick up object
            if (heldObject != null)
            {
                // Reset current held object in order to pick up next object
                DropObject();
                heldObject = null;

                heldObject = objectToHold;
                heldObject.Pickup(holdingPosition.transform);
            }
            else
            {
                heldObject = objectToHold;
                heldObject.Pickup(holdingPosition.transform);
            }
        }

        // Used methods in PlayerController
        public void DropObject()
        {
            if (heldObject != null)
            {
                // A weaker throw so object doesn't sit on player after drop
                heldObject.Throw(holdingPosition.transform.forward, dropForwardForce);
                heldObject = null;
            }
        }

        public void ThrowObject()
        {
            if (heldObject != null)
            {
                heldObject.Throw(holdingPosition.transform.up + transform.forward, throwForce);
                heldObject = null;
            }
        }
    }
}
