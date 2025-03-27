using UnityEngine;

namespace CannonMonke
{
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] float groundDistance = 0.04f;
        [SerializeField] LayerMask groundLayers;

        public bool isGrounded {  get; private set; }

        void Update()
        {
            isGrounded = Physics.SphereCast(
                transform.position,
                groundDistance,
                Vector3.down,
                out _,
                groundDistance,
                groundLayers
                );
        }
    }
}
