using Unity.Cinemachine;
using UnityEngine;

namespace CannonMonke
{
    public class PlatformCollisionHandler : MonoBehaviour 
    {
        Transform platform;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Moving Platform"))
            {
                // If the contact normal is pointing up, player collided with top of platform
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f) return;

                platform = other.transform;
                transform.SetParent(platform);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Moving Platform"))
            {
                transform.SetParent(null);
                platform = null;
            }
        }
    }
}
