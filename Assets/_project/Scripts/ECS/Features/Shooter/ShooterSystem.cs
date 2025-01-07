using System;
using System.Collections.Generic;
using _project.Scripts.Core.Variables.References;
using _project.Scripts.ECS.Features.CameraBoundsDetection;
using _project.Scripts.ECS.Features.Health;
using _project.Scripts.ECS.Features.Movement;
using _project.Scripts.ECS.Features.Spawner;
using _project.Scripts.ECS.Pool;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
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
        
        [SerializeField] [Tooltip("Shoots per second")] private FloatReference attackSpeed;
        
        [SerializeField] private float startBulletSpeedScale;
        [SerializeField] private bool shootAtOneTime;
        
        [SerializeField] [ReadOnly] private float shootInterval;
        [SerializeField] [ReadOnly] private int shootersCount;
        [SerializeField] [ReadOnly] private float cooldown;
        //[SerializeField] [ReadOnly] private bool suspended;
 
        private ComponentPool<MovableProvider> _bulletPool;
        
        private Filter _notEnemyShooterFilter;
        private Stash<Shooter> _shooterStash;

        private Filter _enemiesInGameFieldFilter;
        private Stash<EnemyData> _enemyDataStash;

        private Filter _bulletFilter;
        private Stash<Movable> _movableStash;

        private Stash<HealthComponent> _healthStash;
        
        private Stash<Projectile> _projectileStash;

        private readonly List<ShootRequest> _shootRequestList = new(500);
        
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

            // Найти всех врагов в камере
            _enemiesInGameFieldFilter = World.Filter.
                With<EnemyData>().
                With<InMainCamBounds>().
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

            // Обновляем кулдаун
            cooldown = 1f / attackSpeed;
        }
        
        public override void OnUpdate(float deltaTime)
        {
            UpdateTimers(deltaTime);
            CheckReleaseNeed();
            HandleShootRequests(); // Обрабатываем запросы на выстрел
            if (cooldown > 0) return;
            CreateShootRequests(); // Создать запросы на выстрел
            cooldown = 1f / attackSpeed; // Обновляем кулдаун
        }

        private void HandleShootRequests()
        {
            var requestsToRemove = new List<ShootRequest>();
            
            foreach (var request in _shootRequestList)
            {
                if (request.Delay > 0) continue;
                // Найти направление к ближайшему врагу от запроса
                var shooter = request.Shooter;
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
                    
                    Debug.Log("Bullet Creating Requested", this);
                }
                else
                {
                    Debug.Log("No Target Found For Request", this);
                }
                
                requestsToRemove.Add(request);
            }

            foreach (var request in requestsToRemove)
            {
                _shootRequestList.Remove(request);
                Debug.Log("Shoot Request Removed", this);
            }
        }

        private void CheckReleaseNeed()
        {
            foreach (var entity in _bulletFilter)
            {
                if (entity.IsNullOrDisposed()) continue;
                var release = false;
                var health = _healthStash.Get(entity).HealthPoints; // Получаем хп пули
                // Получаем скаляр скорости пули
                var speedScale = _movableStash.Get(entity).SpeedScale;
                // Если хп пули не больше нуля, выставить релиз флаг
                if (!(health > 0)) release = true;
                // Если скаляр скорости пули не больше нуля, выставить релиз флаг
                if (!(speedScale > 0)) release = true;
                // Перейти к следующей пули, если релиз флаг не выставлен
                if (!release) continue;
                // Получить юнити gameObject
                var gameObject = _projectileStash.Get(entity).Transform.gameObject;
                gameObject.SetActive(false); // Выставить активность
                _bulletPool.Release(gameObject); //
            }
        }
        
        // Единичный выстрел из всех пушек
        private void CreateShootRequests()
        {
            // Узнаём сколько всего пушек
            shootersCount = _shooterStash.Length;
            // Узнаём временной интервал между выстрелами пушек
            shootInterval = 1f / attackSpeed / shootersCount;

            var nextShootDelay = shootInterval;
            
            foreach (var entity in _notEnemyShooterFilter)
            {
                if (entity.IsNullOrDisposed()) continue;
                var shooter = _shooterStash.Get(entity);
                
                var request = new ShootRequest(shooter, nextShootDelay);
                _shootRequestList.Add(request);

                Debug.Log("Created Shoot Request");
                
                if (shootAtOneTime) continue;
                nextShootDelay += shootInterval;
            }
        }
        
        private Vector2 GetNearestToCenterInCircle(Vector2 center, float radius)
        {
            var distanceToNearestTarget = Mathf.Infinity;
            Transform nearestTarget = null;
            
            foreach (var entity in _enemiesInGameFieldFilter)
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
                250, bulletPrefab);
        }

        private void UpdateTimers(float deltaTime)
        {
            cooldown -= deltaTime;
            
            // Уменьшить задержку всех запросов на выстрел
            foreach (var request in _shootRequestList)
            {
                request.Delay -= deltaTime;
            }
        }
    }
}