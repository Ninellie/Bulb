using _project.Scripts.ECS.Features.Health;
using _project.Scripts.ECS.Features.RandomPlacing;
using _project.Scripts.ECS.Pool;
using GameSession.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using TriInspector;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpawnerSystem))]
    public sealed class SpawnerSystem : FixedUpdateSystem
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private ComponentPoolContainer poolContainer;
        
        [Header("Spawn pattern settings")]
        [SerializeField] private SpawnDataPreset spawnData;
        [Header("Readonly Indicators")]
        [SerializeField] [ReadOnly] private int maxEnemies;
        [SerializeField] [ReadOnly] private float spawnQueueSize;
        [SerializeField] [ReadOnly] private float enemiesPerSecond;
        [SerializeField] [ReadOnly] private float timeToNextSpawn;

        private Filter _enemyHealthFilter;
        private Stash<HealthComponent> _healthStash;

        private Stash<EnemyData> _releaseRequestStash;
        
        private ComponentPool<EnemyDataProvider> _enemyPool;

        public override void OnAwake()
        {
            CreatePool();
            _enemyHealthFilter =  World.Filter.With<HealthComponent>().With<EnemyData>().Build();
            _healthStash = World.GetStash<HealthComponent>();
            _releaseRequestStash = World.GetStash<EnemyData>();
            spawnQueueSize = 0;
        }

        public override void OnUpdate(float deltaTime)
        {
            timeToNextSpawn -= deltaTime;
            
            maxEnemies = (int)spawnData.MaxEnemiesOnScreen.Evaluate(Time.timeSinceLevelLoad);
            enemiesPerSecond = maxEnemies / spawnData.FulfillSeconds;
            
            CheckReleaseNeed();
            
            if (timeToNextSpawn <= 0)
            {
                EnqueueSpawn();
            }
            
            if (spawnQueueSize > 0)
            {
                SpawnOneEnemy();
            }
        }

        private void CreatePool()
        {
            _enemyPool = poolContainer.CreatePool<EnemyDataProvider>("Enemy Pool",true, 200, 50, enemyPrefab);
        }

        private void CheckReleaseNeed()
        {
            foreach (var entity in _enemyHealthFilter)
            {
                ref var health = ref _healthStash.Get(entity);
                
                if (health.HealthPoints > 0)
                {
                    continue;
                }
                
                ref var enemyData = ref _releaseRequestStash.Get(entity);
                _enemyPool.Release(enemyData.Transform.gameObject);
            }
        }

        private void SpawnOneEnemy()
        {
            var enemy = _enemyPool.Get();
            var entity = World.CreateEntity();
            ref var enemyPlaceRequest  = ref entity.AddComponent<RandomPlaceRequest>();
            
            enemyPlaceRequest.ObjectTransform = enemy.transform;
            
            spawnQueueSize--;
        }

        private void EnqueueSpawn()
        {
            var isSpawnBlocked = !(_enemyPool.CountActive < maxEnemies);
            if (isSpawnBlocked)
            {
                return;
            }
            spawnQueueSize += enemiesPerSecond * spawnData.IntervalMultiplier;
            timeToNextSpawn = spawnData.IntervalMultiplier;
        }
    }
}