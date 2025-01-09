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

        [SerializeField] [Tooltip("Shoots per second")]
        private FloatReference attackSpeed;

        [SerializeField] private float startBulletSpeedScale;
        [SerializeField] private bool shootAtOneTime;

        [SerializeField] [ReadOnly] private float shootInterval;
        [SerializeField] [ReadOnly] private int shootersCount;
        
        private ComponentPool<MovableProvider> _bulletPool;

        private Filter _aimedReadyShooterFilter;
        private Filter _bulletFilter;
        
        private Stash<Movable> _movableStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<Projectile> _projectileStash;

        public override void OnAwake()
        {
            // Создаём пул пуль
            CreatePool();

            // Найти все прицелившиеся готовые к стрельбе сущности
            _aimedReadyShooterFilter = World.Filter
                .With<Shooter>()
                .With<Aimed>()
                .Without<Cooldown>()
                .Build();

            // Найти все движущиеся снаряды со здоровьем
            _bulletFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<HealthComponent>()
                .Build();
            
            _movableStash = World.GetStash<Movable>();
            _healthStash = World.GetStash<HealthComponent>();
            _projectileStash = World.GetStash<Projectile>();
        }

        public override void OnUpdate(float deltaTime)
        {
            Shoots();
            CheckReleaseNeed();
        }

        private void Shoots()
        {
            var cooldown = 1f / attackSpeed;
            foreach (var entity in _aimedReadyShooterFilter)
            {
                ref var shooter = ref entity.GetComponent<Shooter>();
                ref var aimed = ref entity.GetComponent<Aimed>();
                var shooterPosition = (Vector2)shooter.Transform.position;
                var targetPosition = (Vector2)aimed.Target.position;

                // Создать запрос на создание пули (пока просто создать пулю)
                var bulletMovableProvider = _bulletPool.Get();
                ref var bulletMovable = ref _movableStash.Get(bulletMovableProvider.Entity);
                bulletMovable.SpeedScale = startBulletSpeedScale;
                bulletMovable.Transform.position = shooterPosition;
                bulletMovable.Direction.constantValue = (targetPosition - shooterPosition).normalized;

                entity.AddComponent<Cooldown>().Current = cooldown;
                entity.RemoveComponent<Aimed>();
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

        private void CreatePool()
        {
            _bulletPool = poolContainer.CreatePool<MovableProvider>(
                "Bullet Pool", true, 200,
                250, bulletPrefab);
        }
    }
}