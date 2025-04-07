using System;
using UnityEngine;

namespace CannonMonke
{
    [CreateAssetMenu(fileName = "Float Event", menuName = "CannonMonke/Float Event")]
    public class FloatEventSO : ScriptableObject
    {
        public event Action<float> listeners;

        public void Raise(float value)
        {
            listeners?.Invoke(value);
        }

        public void RegisterListener(Action<float> listener)
        {
            listeners += listener;
        }

        public void UnregisterListener(Action<float> listener)
        {
            listeners -= listener;
        }
    }
}
