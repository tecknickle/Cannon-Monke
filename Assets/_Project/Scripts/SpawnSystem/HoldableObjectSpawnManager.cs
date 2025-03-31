using UnityEngine;
using Utilities;

namespace CannonMonke
{
    public class HoldableObjectSpawnManager : EntitySpawnManager
    {
        [SerializeField] HoldableObjectData[] holdableObjectData;
        [SerializeField] float spawnInterval = 0f;

        EntitySpawner<HoldableObject> spawner;

        CountdownTimer spawnTimer;
        int counter;

        protected override void Awake()
        {
            base.Awake();

            spawner = new EntitySpawner<HoldableObject>(
                new EntityFactory<HoldableObject>(holdableObjectData),
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
