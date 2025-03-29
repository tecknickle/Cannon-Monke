using UnityEngine;

namespace CannonMonke
{
    public abstract class EntitySpawnManager : MonoBehaviour 
    {
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField] protected Transform[] spawnPoints;

        protected ISpawnPointStrategy spawnPointStrategy;

        protected enum SpawnPointStrategyType
        {
            Linear,
            Random
        }

        protected virtual void Awake()
        {
            switch (spawnPointStrategyType)
            {
                case SpawnPointStrategyType.Linear:
                    spawnPointStrategy = new LinearSpawnPointStrategy(spawnPoints);
                    break;
                case SpawnPointStrategyType.Random:
                    spawnPointStrategy = new RandomSpawnPointStrategy(spawnPoints);
                    break;
            }
        }

        public abstract void Spawn();
    }
}
