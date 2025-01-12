using _project.Scripts.ECS.Features.Aiming;
using _project.Scripts.ECS.Features.CameraBoundsDetection;
using _project.Scripts.ECS.Features.CooldownReduction;
using _project.Scripts.ECS.Features.EnergyConsumption;
using _project.Scripts.ECS.Features.Health;
using _project.Scripts.ECS.Features.Movement;
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

        [SerializeField] private float startBulletSpeedScale;
        [SerializeField] private bool shootAtOneTime;

        [SerializeField] [ReadOnly] private float shootInterval;
        [SerializeField] [ReadOnly] private int shootersCount;
        
        private ComponentPool<MovableProvider> _bulletPool;

        private Filter _aimedReadyShooterFilter;
        
        private Filter _bulletFilter;
        private Filter _bulletOutOfCamFilter;
        
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
                .Without<EnergyEmpty>()
                .With<Aimed>()
                .Without<Cooldown>()
                .Build();

            // Найти все движущиеся снаряды со здоровьем
            _bulletFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<HealthComponent>()
                .With<InMainCamBounds>()
                .Build();
            
            // Найти пули за камерой
            _bulletOutOfCamFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<HealthComponent>()
                .Without<InMainCamBounds>()
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
            foreach (var entity in _aimedReadyShooterFilter)
            {
                ref var energyReserve = ref entity.GetComponent<EnergyReserve>();
                ref var shooter = ref entity.GetComponent<Shooter>();
                ref var aimed = ref entity.GetComponent<Aimed>();
                var cost = shooter.Cost.Value;

                if (energyReserve.CurrentAmount - cost < 0)
                {
                    // Недостаточно энергии, выключить и наложить штрафной длительный кд
                    // todo сделать визуальный эффект выключения, снизить излучаемую яркость до нуля
                    
                    entity.AddComponent<Cooldown>().Current = shooter.PenaltyTime;
                    entity.RemoveComponent<Aimed>();
                    continue;
                }
                
                energyReserve.CurrentAmount -= cost;
                var shooterPosition = (Vector2)shooter.Transform.position;
                var targetPosition = (Vector2)aimed.Target.position;

                entity.AddComponent<Cooldown>().Current = shooter.Cooldown;
                entity.RemoveComponent<Aimed>();
                
                // Создать запрос на создание пули (пока просто создать пулю)
                CreateBullet(shooterPosition, targetPosition);
            }
        }

        private void CreateBullet(Vector2 shooterPosition, Vector2 targetPosition)
        {
            var bulletMovableProvider = _bulletPool.Get();
            ref var bulletMovable = ref _movableStash.Get(bulletMovableProvider.Entity);
            bulletMovable.SpeedScale = startBulletSpeedScale;
            bulletMovable.Transform.position = shooterPosition;
            bulletMovable.Direction.constantValue = (targetPosition - shooterPosition).normalized;
        }

        private void CheckReleaseNeed()
        {
            foreach (var entity in _bulletOutOfCamFilter)
            {
                if (entity.IsNullOrDisposed()) continue;
                ReleaseBullet(entity);
            }
            
            foreach (var entity in _bulletFilter)
            {
                if (entity.IsNullOrDisposed()) continue;
                var health = _healthStash.Get(entity).HealthPoints; // Получаем хп пули
                if (!(health > 0)) // Если хп пули не больше нуля, выставить релиз флаг
                {
                    ReleaseBullet(entity);
                    continue;
                };
                var speedScale = _movableStash.Get(entity).SpeedScale; // Получаем скаляр скорости пули
                if (!(speedScale > 0)) // Если скаляр скорости пули не больше нуля, выставить релиз флаг
                {
                    ReleaseBullet(entity);
                }
            }
        }

        private void ReleaseBullet(Entity entity)
        {
            var gameObject = _projectileStash.Get(entity).Transform.gameObject;
            gameObject.SetActive(false); // Выставить активность
            _bulletPool.Release(gameObject); // todo жижа каждый раз делает GetComponent внутри 
            entity.Dispose();
        }

        private void CreatePool()
        {
            _bulletPool = poolContainer.CreatePool<MovableProvider>(
                "Bullet Pool", true, 200,
                250, bulletPrefab);
        }
    }


    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(ProjectileHandlerSystem))]
    public sealed class ProjectileHandlerSystem : FixedUpdateSystem
    {
        public override void OnAwake()
        {
            
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }
    }
}