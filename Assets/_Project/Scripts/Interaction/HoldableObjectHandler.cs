using UnityEngine;

namespace CannonMonke
{
    public class HoldableObjectHandler : MonoBehaviour, IInteractable, IHoldable
    {
        Transform holder;
        Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (holder != null)
            {
                // Keeps object in holding position every frame
                holder.GetPositionAndRotation(out Vector3 targetPosition, out Quaternion targetRotation);
                transform.SetPositionAndRotation(targetPosition, targetRotation);
            }
        }

        public void Interact(Transform interactor)
        {
            // Tells player to try to pick up this object
            if (interactor.TryGetComponent(out PlayerObjectHolding player))
            {
                player.PickUpObject(this);
            }
        }

        public void Pickup(Transform newHolder)
        {
            // Sets player as holder of this object
            holder = newHolder;
            // Prevents physics from affecting object while held
            rb.isKinematic = true;
        }

        public void Drop()
        {
            holder = null;
            rb.isKinematic = false;
        }

        public void Throw(Vector3 direction, float force)
        {
            Drop();
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}
