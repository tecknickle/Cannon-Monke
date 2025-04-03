using KBCore.Refs;
using UnityEngine;
using UnityEngine.Rendering;

namespace CannonMonke
{
    public class Interactor : MonoBehaviour
    {
        [Header("Interact Settings")]
        [SerializeField] float interactRange = 3f;
        [SerializeField] bool isDebugging;
        [SerializeField] Vector3 rayPositionOffset = new(0f, 1f, 0f);

        void Start()
        {
            isDebugging = false;
        }

        void Update()
        {
            if (isDebugging)
            {
                Debug.DrawRay(
                    transform.position + rayPositionOffset, 
                    transform.forward * interactRange, 
                    Color.red);
            }
        }

        public void DoInteraction()
        {
            Ray ray = new(transform.position + rayPositionOffset, transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
            {
                Debug.Log("Hit Object: " + hitInfo.collider.gameObject.name);

                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    Debug.Log("Interactable Obj found: " + hitInfo.collider.gameObject.name);
                    interactable.Interact(transform);
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
