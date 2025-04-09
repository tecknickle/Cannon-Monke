using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class HoldableSpawnManager : EntitySpawnManager
    {
        [SerializeField] HoldableData[] holdableObjectData;
        [SerializeField] float spawnInterval = 0f;

        EntitySpawner<Holdable> spawner;

        CountdownTimer spawnTimer;
        int counter;

        protected override void Awake()
        {
            base.Awake();

            spawner = new EntitySpawner<Holdable>(
                new EntityFactory<Holdable>(holdableObjectData),
                spawnPointStrategy);

            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () =>
            {
                if (counter++ >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }

                Spawn();
                spawnTimer.Start();
            };
        }

        void Start() => spawnTimer.Start();

        void Update() => spawnTimer.Tick(Time.deltaTime);

        public override void Spawn() => spawner.Spawn();
    }
}
