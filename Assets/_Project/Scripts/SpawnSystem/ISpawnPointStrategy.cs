using NUnit.Framework;
using UnityEngine;

namespace CannonMonke
{
    public interface ISpawnPointStrategy 
    {
        Transform NextSpawnPoint();
    }
}
