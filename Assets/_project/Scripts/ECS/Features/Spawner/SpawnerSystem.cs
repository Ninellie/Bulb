using _project.Scripts.ECS.Features.RandomPlacing;
using Core.Shapes;
using GameSession.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(SpawnerSystem))]
    public sealed class SpawnerSystem : FixedUpdateSystem
    {
        [SerializeField] private GameObject enemyPrefab;
        
        [Header("Spawn pattern settings")]
        [SerializeField] private SpawnDataPreset spawnData;
        [Header("Readonly Indicators")]
        [SerializeField] [TriInspector.ReadOnly] private int maxEnemies;
        [SerializeField] [TriInspector.ReadOnly] private float spawnQueueSize;
        [SerializeField] [TriInspector.ReadOnly] private float enemiesPerSecond;
        [SerializeField] [TriInspector.ReadOnly] private float timeToNextSpawn;

        private EnemyDataComponentPool _enemyPool;
        
        private Filter _filter;
        private Stash<EnemyData> _releaseRequestStash;
        
        public override void OnAwake()
        {
            CreatePool();
            _filter =  World.Filter.With<ReleaseRequest>().With<EnemyData>().Build();
            _releaseRequestStash = World.GetStash<EnemyData>();
            spawnQueueSize = 0;
        }

        public override void OnUpdate(float deltaTime)
        {
            timeToNextSpawn -= deltaTime;
            
            maxEnemies = (int)spawnData.MaxEnemiesOnScreen.Evaluate(Time.timeSinceLevelLoad);
            enemiesPerSecond = maxEnemies / spawnData.FulfillSeconds;
            
            CheckReleaseRequests();
            
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
            var root = new GameObject("Enemy Pool")
            {
                transform =
                {
                    position = Vector3.zero
                }
            };
            
            _enemyPool = 
                new EnemyDataComponentPool(true, 200, 50, root, enemyPrefab);
            _enemyPool.Init();
        }

        private void CheckReleaseRequests()
        {
            foreach (var entity in _filter)
            {
                entity.RemoveComponent<ReleaseRequest>();
                _enemyPool.Release(_releaseRequestStash.Get(entity).Transform.gameObject);
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