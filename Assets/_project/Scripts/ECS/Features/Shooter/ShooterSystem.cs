using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Variables.References;
using _project.Scripts.ECS.Features.Aiming;
using _project.Scripts.ECS.Features.CooldownReduction;
using _project.Scripts.ECS.Features.Health;
using _project.Scripts.ECS.Features.Movement;
using _project.Scripts.ECS.Features.Spawner;
using _project.Scripts.ECS.Pool;
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
        
        [SerializeField] [Tooltip("Shoots per second")] private FloatReference attackSpeed;
        
        [SerializeField] private float startBulletSpeedScale;
        [SerializeField] private bool shootAtOneTime;
        
        [SerializeField] [ReadOnly] private float shootInterval;
        [SerializeField] [ReadOnly] private int shootersCount;
        [SerializeField] [ReadOnly] private float cooldown;
        //[SerializeField] [ReadOnly] private bool suspended;
 
        private ComponentPool<MovableProvider> _bulletPool;
        
        private Filter _notEnemyAimedShooterFilter;
        private Stash<Shooter> _shooterStash;
        
        private Stash<Aimed> _aimedStash;

        private Filter _bulletFilter;
        private Stash<Movable> _movableStash;

        private Stash<HealthComponent> _healthStash;
        
        private Stash<Projectile> _projectileStash;

        private readonly List<ShootRequest> _shootRequestList = new(500);
        
        public override void OnAwake()
        {
            // Создаём пул пуль
            CreatePool();
            
            // Найти все прицелившиеся готовые к стрельбе сущности
            _notEnemyAimedShooterFilter = World.Filter
                .With<Shooter>()
                .With<Aimed>()
                .Without<Cooldown>()
                .Build();
            
            _shooterStash = World.GetStash<Shooter>();

            // Найти все движущиеся снаряды со здоровьем
            _bulletFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<HealthComponent>()
                .Build();
            
            _aimedStash = World.GetStash<Aimed>();
            _movableStash = World.GetStash<Movable>();
            _healthStash = World.GetStash<HealthComponent>();
            _projectileStash = World.GetStash<Projectile>();

            // Обновляем время восстановления
            cooldown = 1f / attackSpeed;
        }
        
        public override void OnUpdate(float deltaTime)
        {
            UpdateTimers(deltaTime);
            CheckReleaseNeed();
            HandleShootRequests(); // Обрабатываем запросы на выстрел
            if (cooldown > 0) return;
            CreateShootRequests(); // Создать запросы на выстрел
            cooldown = 1f / attackSpeed; // Обновляем время восстановления
        }
        
        private void HandleShootRequests()
        {
            var requestsToRemove = new List<ShootRequest>();
            
            foreach (var request in _shootRequestList.Where(request => !(request.Delay > 0)))
            {
                // Проверить прицелен ли шутер
                if (!request.ShooterEntity.IsNullOrDisposed())
                {
                    if (_aimedStash.Has(request.ShooterEntity))
                    {
                        // Найти направление к ближайшему врагу от запроса
                        ref var shooter = ref request.ShooterEntity.GetComponent<Shooter>();
                        ref var aimed = ref request.ShooterEntity.GetComponent<Aimed>();
                        var shooterPosition = (Vector2)shooter.Transform.position;
                        var targetPosition = (Vector2)aimed.Target.position;
                        
                        // Создать запрос на создание пули (пока просто создать пулю)
                        var bulletMovableProvider = _bulletPool.Get();
                        ref var bulletMovable = ref _movableStash.Get(bulletMovableProvider.Entity);
                        bulletMovable.SpeedScale = startBulletSpeedScale;
                        bulletMovable.Transform.position = shooterPosition;
                        bulletMovable.Direction.constantValue = (targetPosition - shooterPosition).normalized;
                        Debug.Log("Bullet Creating Requested", this);
                    }
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
                _bulletPool.Release(gameObject); // todo жижа каждый раз делает GetComponent внутри
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
            
            foreach (var entity in _notEnemyAimedShooterFilter)
            {
                if (entity.IsNullOrDisposed()) continue;
                _shootRequestList.Add(new ShootRequest(entity, nextShootDelay));
                Debug.Log("Created Shoot Request");
                if (shootAtOneTime) continue;
                nextShootDelay += shootInterval;
            }
        }
        
        private void CreatePool()
        {
            _bulletPool = poolContainer.CreatePool<MovableProvider>(
                "Bullet Pool",true, 200,
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