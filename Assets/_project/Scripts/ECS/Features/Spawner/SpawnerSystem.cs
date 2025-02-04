using _project.Scripts.ECS.Features.HealthChanging;
using _project.Scripts.ECS.Features.RandomPlacing;
using _project.Scripts.ECS.Pool;
using GameSession.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using TriInspector;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(SpawnerSystem))]
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

        private ComponentPool<EnemyDataProvider> _enemyPool;
        
        private Filter _enemyHealthFilter;
        
        private Stash<Health> _healthStash;
        private Stash<EnemyData> _enemyDataStash;
        private Stash<EnemyReturnToPoolEvent> _returnToPoolEventStash;
        
        public override void OnAwake()
        {
            CreatePool();
            
            _enemyHealthFilter =  World.Filter
                .With<Health>()
                .With<EnemyData>()
                .Build();
            
            _healthStash = World.GetStash<Health>();
            _enemyDataStash = World.GetStash<EnemyData>();
            _returnToPoolEventStash = World.GetStash<EnemyReturnToPoolEvent>();
            
            spawnQueueSize = 0;
        }
        
        public override void OnUpdate(float deltaTime)
        {
            _returnToPoolEventStash.RemoveAll();
            
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
            _enemyPool = poolContainer
                .CreatePool<EnemyDataProvider>("Enemy Pool",true, 200, 50, enemyPrefab);
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
                
                var eventHolderEntity = World.CreateEntity();
                _returnToPoolEventStash.Add(eventHolderEntity);
                
                ref var enemyData = ref _enemyDataStash.Get(entity);
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