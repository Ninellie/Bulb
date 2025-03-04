using _project.Scripts.ECS.Features.CameraBoundsDetection;
using _project.Scripts.ECS.Features.HealthChanging;
using _project.Scripts.ECS.Features.Moving;
using _project.Scripts.ECS.Pool;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Projectiles
{
    /// <summary>
    /// Создаёт снаряды, отслеживает необходимость их освобождения и освобождает
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(ProjectileHandlerSystem))]
    public sealed class ProjectileHandlerSystem : FixedUpdateSystem
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ComponentPoolContainer poolContainer;
        
        private ComponentPool<MovableProvider> _bulletPool;
        
        private Filter _bulletFilter;
        private Filter _bulletOutOfCamFilter;
        
        private Stash<Health> _healthStash;
        private Stash<Projectile> _projectileStash;
        private Stash<Movable> _movableStash;
        
        private Stash<CreateProjectileRequest> _projectileCreateRequestsStash;
        
        public override void OnAwake()
        {
            // Создаём пул пуль
            CreatePool();
            
            // Найти пули за камерой
            _bulletOutOfCamFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<Health>()
                .Without<InMainCamBounds>()
                .Build();
            
            // Найти все движущиеся снаряды со здоровьем
            _bulletFilter = World.Filter
                .With<Projectile>()
                .With<Movable>()
                .With<Health>()
                .With<InMainCamBounds>()
                .Build();
            
            _healthStash = World.GetStash<Health>();
            _projectileStash = World.GetStash<Projectile>();
            _movableStash = World.GetStash<Movable>();
            _projectileCreateRequestsStash = World.GetStash<CreateProjectileRequest>();
        }

        public override void OnUpdate(float deltaTime)
        {
            CheckReleaseNeed();
            HandleCreateProjectileRequests();
        }

        private void HandleCreateProjectileRequests()
        {
            foreach (var request in _projectileCreateRequestsStash)
            {
                var initialPosition = request.InitialPosition;
                var targetPosition = request.TargetPosition;
                var speedScale = request.StartBulletSpeedScale;
                
                var bulletMovableProvider = _bulletPool.Get();
                
                ref var bulletMovable = ref _movableStash.Get(bulletMovableProvider.Entity);
                
                bulletMovable.SpeedScale = speedScale;
                bulletMovable.Transform.position = initialPosition;
                
                var direction =(targetPosition - initialPosition).normalized;

                bulletMovable.Direction = direction;
            }
            _projectileCreateRequestsStash.RemoveAll();
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
                
                if (health <= 0) // Если хп пули меньше или равно нулю освободить пулю
                {
                    ReleaseBullet(entity);
                    continue;
                }
                
                // Получаем скаляр скорости пули
                var speedScale = _movableStash.Get(entity).SpeedScale; 
                
                // Если скаляр скорости пули меньше или равно нулю освободить пулю
                if (speedScale <= 0)
                {
                    ReleaseBullet(entity);
                }
            }
        }
        
        private void ReleaseBullet(Entity entity)
        {
            var movableProvider = _projectileStash.Get(entity).MovableProvider;
            movableProvider.gameObject.SetActive(false); // Выключить игровой объект
            _bulletPool.Release(movableProvider);
            entity.Dispose();
        }

        private void CreatePool()
        {
            _bulletPool = poolContainer.CreatePool<MovableProvider>(
                "Bullet Pool", true, 200,
                250, bulletPrefab);
        }
    }
}