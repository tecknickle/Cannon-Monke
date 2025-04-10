using System;
using UnityEngine;

namespace CannonMonke
{
    public interface IOrdinance
    {
        void EnableOrdinance();
        void OnCollisionEnter(Collision collision);
        void OnHit();
    }
}

