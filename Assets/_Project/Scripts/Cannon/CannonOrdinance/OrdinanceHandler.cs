using KBCore.Refs;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CannonMonke
{
    public class OrdinanceHandler : MonoBehaviour
    {
        public void HandleNewOrdinance(Transform newObject)
        {
            if (newObject == null) return;
            if (newObject.TryGetComponent(out IOrdinance ordinance))
            {
                ordinance.EnableOrdinance();
            }
            else
            {
                Debug.LogWarning("New object does not have an ordinance component.");
            }
        }
    }
}

