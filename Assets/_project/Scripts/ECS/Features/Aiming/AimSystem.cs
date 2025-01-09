using _project.Scripts.ECS.Features.CameraBoundsDetection;
using _project.Scripts.ECS.Features.CooldownReduction;
using _project.Scripts.ECS.Features.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Aiming
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(AimSystem))]
    public sealed class AimSystem : FixedUpdateSystem
    {
        private Filter _enemiesInCamBoundsFilter;
        private Stash<EnemyData> _enemyDataStash;
        
        private Filter _aimingFilter;
        
        private Stash<Aimed> _aimedStash;

        public override void OnAwake()
        {
            // Найти всех врагов в камере
            _enemiesInCamBoundsFilter = World.Filter
                .With<EnemyData>()
                .With<InMainCamBounds>()
                .Build();
            
            _enemyDataStash = World.GetStash<EnemyData>();

            _aimedStash = World.GetStash<Aimed>();

            _aimingFilter = World.Filter.With<Aiming>()
                .Without<Cooldown>()
                .Build();
        }
        
        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _aimingFilter)
            {
                ref var aiming = ref entity.GetComponent<Aiming>();
                Vector2 aimingPos = aiming.Transform.position;
                var radius = aiming.AimingRadius;
                
                // Найти ближайшую цель, если она есть
                var nearestEnemy = GetNearestToCenterInCircle(aimingPos, radius);
                var isAimed = _aimedStash.Has(entity);
                var withoutEnemy = nearestEnemy is null;

                switch (isAimed)
                {
                    // Не целится и врагов поблизости нет
                    case false when withoutEnemy:
                        continue;
                    case true when withoutEnemy:
                        _aimedStash.Remove(entity); // Если цель прицелена, удалить компонент
                        break;
                }
                switch (isAimed)
                {
                    // Если цель прицелена и враг есть, обновить цель
                    case true when !withoutEnemy:
                    {
                        ref var aimed = ref _aimedStash.Get(entity);
                        aimed.Target = nearestEnemy;
                        break;
                    }
                    case false:
                    {
                        // Добавить компонент прицеливания
                        ref var aimed = ref entity.AddComponent<Aimed>();
                        aimed.Target = nearestEnemy;
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Среди врагов в границах главной камеры производит поиск ближайшего к центру указанной области
        /// </summary>
        /// <param name="center">Центр области поиска</param>
        /// <param name="radius">Радиус поиска</param>
        /// <returns>Vector2 - позиция ближайшего врага</returns>
        private Transform GetNearestToCenterInCircle(Vector2 center, float radius)
        {
            var distanceToNearestTarget = Mathf.Infinity;
            Transform nearestTarget = null;
            
            foreach (var entity in _enemiesInCamBoundsFilter)
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

            return nearestTarget;
        }
    }
}