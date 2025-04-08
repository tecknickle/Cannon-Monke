using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;

namespace CannonMonke
{
    public class OrdinanceHandler : MonoBehaviour
    {
        public void GiveProjectileOrdinance(Transform projectile)
        {
            if (projectile.TryGetComponent(out ExplosiveComponent explosive))
            {
                explosive.EnableExplosive();
            }
            else
            {
                projectile.AddComponent<ExplosiveComponent>().EnableExplosive();
            }
        } 
    }
}

