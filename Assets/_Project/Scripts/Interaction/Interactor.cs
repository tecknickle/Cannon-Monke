using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering;

namespace CannonMonke
{
    public class Interactor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField, Anywhere] Transform interactorSource;

        [Header("Settings")]
        [SerializeField] float interactRange = 3f;

        private void Update()
        {
            Debug.DrawRay(interactorSource.position, interactorSource.forward * interactRange, Color.red);
        }

        public void HandleInteraction()
        {
            Ray ray = new(interactorSource.position, interactorSource.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
            {
                Debug.Log("Hit Object: " + hitInfo.collider.gameObject.name);

                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    Debug.Log("Interactable Obj found: " + hitInfo.collider.gameObject.name);
                    interactable.Interact();
                }
                else
                {
                    Debug.Log("No interactable object found.");
                }
            }
            else
            {
                Debug.Log("Raycast hit nothing");
            }
        }    
    }
}
