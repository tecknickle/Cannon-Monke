using UnityEngine;

namespace CannonMonke
{
    public class GroundChecker : MonoBehaviour
    {
        [Header("Ground Check Settings")]
        [SerializeField] float groundDistance = 1f;
        [SerializeField] Vector3 rayOriginOffset = new(0f, 0.5f, 0f);
        [SerializeField] LayerMask groundLayers;

        public bool IsGrounded {  get; private set; }

        void Update()
        {
            IsGrounded = Physics.Raycast(transform.position + rayOriginOffset, Vector3.down, groundDistance);

            Debug.DrawRay(
                transform.position, 
                Vector3.down * groundDistance, 
                IsGrounded ? Color.green : Color.red);
     
        }
    }
}
