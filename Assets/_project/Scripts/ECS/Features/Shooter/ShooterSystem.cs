using _project.Scripts.ECS.Features.Health;
using _project.Scripts.ECS.Features.Movement;
using _project.Scripts.ECS.Features.Spawner;
using _project.Scripts.ECS.Features.Visability;
using _project.Scripts.ECS.Pool;
using Core.Variables.References;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using TriInspector;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(ShooterSystem))]
    public sealed class ShooterSystem : FixedUpdateSystem
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ComponentPoolContainer poolContainer;
        
        [SerializeField] private FloatReference attackSpeed; // Выстрелы в секунду
        
        [SerializeField] private float bulletSpeedScaleDecreasePerSecond;
        [SerializeField] private float startBulletSpeedScale;

        [SerializeField] private bool shootAtOneTime;
        [SerializeField] [ReadOnly] private float timeBetweenShoots;
        [SerializeField] [ReadOnly] private int shootersCount;
        [SerializeField] [ReadOnly] private float cooldown;
        [SerializeField] [ReadOnly] private bool suspended;
 
        private ComponentPool<MovableProvider> _bulletPool;
        
        private Filter _notEnemyShooterFilter;
        private Stash<Shooter> _shooterStash;

        private Filter _shootRequestsFilter;
        private Stash<ShootRequest> _shootRequestStash;

        private Filter _visibleEnemiesFilter;
        private Stash<EnemyData> _enemyDataStash;

        private Filter _bulletFilter;
        private Stash<Movable> _movableStash;

        private Stash<HealthComponent> _healthStash;
        
        private Stash<Projectile> _projectileStash;
        
        private Stash<Visible> _visibleStash;
        private Stash<Invisible> _invisibleStash;
        
        // Можно создать несколько систем стрельбы, но лучше будет как-то переделать суть.
        public override void OnAwake()
        {
            // Создаём пул пуль
            CreatePool();
            
            // Найти все стреляющие сущности, которые не являются врагами
            _notEnemyShooterFilter = World.Filter.
                With<Shooter>().
                Without<EnemyData>().
                Build();
            _shooterStash = World.GetStash<Shooter>();

            // Найти все запросы на выстрел
            _shootRequestsFilter = World.Filter.
                With<ShootRequest>().
                Build();
            _shootRequestStash = World.GetStash<ShootRequest>();

            // Найти всех враов
            _visibleEnemiesFilter = World.Filter.
                With<EnemyData>().
                With<Visible>().
                Without<Invisible>().
                Build();
            _enemyDataStash = World.GetStash<EnemyData>();

            // Найти все движущиеся снаряды со здоровьем
            _bulletFilter = World.Filter.
                With<Projectile>().
                With<Movable>().
                With<HealthComponent>().
                Build();
            _movableStash = World.GetStash<Movable>();
            _healthStash = World.GetStash<HealthComponent>();
            _projectileStash = World.GetStash<Projectile>();
            
            _visibleStash = World.GetStash<Visible>();
            _invisibleStash = World.GetStash<Invisible>();
            
            // Обновляем кулдаун
            cooldown = 1f / attackSpeed;
        }
        
        public override void OnUpdate(float deltaTime)
        {
            // Ловим все компоненты суспенда и суспендим если находим и удаляем компоненты
            // Ловим все компоненты возобновления и возобновляем если находим и удаляем компоненты   
            UpdateTimers(deltaTime);
            
            // Обновляем скаляры скоростей пуль
            // todo перенести в MovementSystem
            DecreaseBulletsSpeedScale(deltaTime);
            
            CheckReleaseNeed();
            
            // Обрабатываем запросы на выстрел
            HandleShootRequests();

            if (_visibleEnemiesFilter.IsEmpty())
            {
                return;
            }

            if (cooldown > 0) return;
            
            // Создать запросы на выстрел
            CreateShootRequests();
                
            // Обновляем кулдаун
            cooldown = 1f / attackSpeed;
        }

        private void DecreaseBulletsSpeedScale(float deltaTime)
        {
            foreach (var entity in _bulletFilter)
            {
                if (entity.IsNullOrDisposed())
                {
                    continue;
                }

                if (!_movableStash.Has(entity))
                {
                    return;
                }
                ref var movable = ref _movableStash.Get(entity);
                movable.SpeedScale -= bulletSpeedScaleDecreasePerSecond * deltaTime;
            }
        }

        private void HandleShootRequests()
        {
            foreach (var entity in _shootRequestsFilter)
            {
                if (entity.IsNullOrDisposed())
                {
                    continue;
                }
                ref var shootRequest = ref _shootRequestStash.Get(entity);
                if (shootRequest.delay > 0)
                {
                    continue;
                }
                
                // Найти направление к ближайшему врагу от запроса
                ref var shooter = ref shootRequest.shooter;
                var shooterPosition = (Vector2)shooter.Transform.position;
                var targetPosition = GetNearestToPosition(shooterPosition);

                if (!targetPosition.Equals(Vector2.zero))
                {
                    // Создать запрос на создание пули (пока просто создать пулю)
                    var bulletMovableProvider = _bulletPool.Get();
                    
                    ref var bulletMovable = ref _movableStash.Get(bulletMovableProvider.Entity);
                    bulletMovable.SpeedScale = startBulletSpeedScale;
                    bulletMovable.Transform.position = shooterPosition;
                    bulletMovable.Direction.constantValue = (targetPosition - shooterPosition).normalized;
                }
                
                _shootRequestStash.Remove(entity);
            }
        }

        private void CheckReleaseNeed()
        {
            foreach (var entity in _bulletFilter)
            {
                if (entity.IsNullOrDisposed())
                {
                    continue;
                }

                var release = _invisibleStash.Has(entity);

                // Получаем хп пули
                var health = _healthStash.Get(entity).HealthPoints;
                // Получаем скаляр скорости пули
                var speedScale = _movableStash.Get(entity).SpeedScale;

                // Если хп пули не больше нуля, то полчить проджектайл и вернуть в пул и перейти
                // к следующей пуле
                if (!(health > 0))
                {
                    release = true;
                }
                
                // Если скаляр скорости пули не больше нуля, то полчить проджектайл и вернуть в пул и перейти
                // к следующей пуле
                if (!(speedScale > 0))
                {
                    release = true;
                }

                if (!release) continue;
                ref var projectile = ref _projectileStash.Get(entity);
                _bulletPool.Release(projectile.Transform.gameObject);
            }
        }
        
        // Единичный выстрел из всех пушек
        private void CreateShootRequests()
        {
            shootersCount = _shooterStash.Length;
            timeBetweenShoots = 1f / attackSpeed / shootersCount;

            var nextShootDelay = 0f;
            
            foreach (var entity in _notEnemyShooterFilter)
            {
                if (entity.IsNullOrDisposed())
                {
                    continue;
                }
                var requestEntity = World.CreateEntity();
                ref var request = ref _shootRequestStash.Add(requestEntity);
                
                var shooter = _shooterStash.Get(entity);
                request.shooter = shooter;
                
                request.delay = nextShootDelay;
                
                if (shootAtOneTime)
                {
                    continue;
                }
                
                nextShootDelay += timeBetweenShoots;
            }
        }
        
        private Vector2 GetNearestToCenterInCircle(Vector2 center, float radius)
        {
            var distanceToNearestTarget = Mathf.Infinity;
            Transform nearestTarget = null;
            foreach (var entity in _visibleEnemiesFilter)
            {
                ref var target = ref _enemyDataStash.Get(entity);
                var transform = target.Transform;
                var direction = (Vector2)transform.position;
                var distance = Vector2.Distance(center, direction);
                if (!(distance < radius)) continue;
                if (!(distance < distanceToNearestTarget)) continue;
                distanceToNearestTarget = distance;
                nearestTarget = transform;
            }

            if (nearestTarget != null) return nearestTarget.position;

            return Vector2.zero;
        }

        private Vector2 GetNearestToPosition(Vector2 position)
        {
            return GetNearestToCenterInCircle(position, Mathf.Infinity);
        }
        
        private void CreatePool()
        {
            _bulletPool = poolContainer.CreatePool<MovableProvider>("Bullet Pool",true, 200,
                50, bulletPrefab);
        }

        private void UpdateTimers(float deltaTime)
        {
            cooldown -= deltaTime;
            // Уменьшить задержку всех запросов на выстрел
            foreach (var entity in _shootRequestsFilter)
            {
                ref var shootRequest = ref _shootRequestStash.Get(entity);
                shootRequest.delay -= deltaTime;
            }
        }
    }
}