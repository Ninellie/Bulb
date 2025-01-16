using _project.Scripts.ECS.Features.Aiming;
using _project.Scripts.ECS.Features.CooldownReduction;
using _project.Scripts.ECS.Features.EnergyConsumption;
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
        [SerializeField] private float startBulletSpeedScale;
        private Filter _readyToShootFilter;
        
        public override void OnAwake()
        {
            // Найти все сущности готовые к стрельбе
            _readyToShootFilter = World.Filter
                .With<Aimed>() // Прицелившиеся
                .With<Shooter>() // Стрелки
                .Without<Cooldown>() // Не на перезарядке
                .Without<EnergyEmpty>() // У которых не пустая энергия
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _readyToShootFilter)
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
                
                // Создать запрос на создание пули
                ref var request = ref World.CreateEntity().AddComponent<CreateProjectileRequest>();
                request.InitialPosition = shooterPosition;
                request.TargetPosition = targetPosition;
                request.StartBulletSpeedScale = startBulletSpeedScale;
            }
        }
    }
}