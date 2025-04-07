using System;
using UnityEngine;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "Void Event", menuName = "CannonMonke/Void Event")]
    public class VoidEventSO : ScriptableObject 
    {
        public event Action listeners;

        public void Raise()
        {
            listeners?.Invoke();
        }

        public void RegisterListener(Action listener)
        {
            listeners += listener;
        }

        public void UnregisterListener(Action listener)
        {
            listeners -= listener;
        }

    }
}
