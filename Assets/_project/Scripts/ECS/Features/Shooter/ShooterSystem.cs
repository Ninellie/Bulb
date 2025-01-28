using _project.Scripts.ECS.Features.Aiming;
using _project.Scripts.ECS.Features.CooldownReduction;
using _project.Scripts.ECS.Features.EnergyConsumption;
using _project.Scripts.ECS.Features.Projectiles;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
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
            _readyToShootFilter = World.Filter
                .With<Aimed>()
                .With<Shooter>()
                .Without<Cooldown>()
                .Without<EnergyEmpty>()
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _readyToShootFilter)
            {
                ref var energyReserve = ref entity.GetComponent<EnergyReserve>();
                ref var shooter = ref entity.GetComponent<Shooter>();
                ref var aimed = ref entity.GetComponent<Aimed>();
                
                var cost = shooter.Cost;

                if (energyReserve.CurrentAmount - cost < 0)
                {
                    entity.AddComponent<Cooldown>().Current = shooter.PenaltyTime;
                    entity.RemoveComponent<Aimed>();
                    continue;
                }
                
                energyReserve.CurrentAmount -= cost;
                var shooterPosition = (Vector2)shooter.Transform.position;
                var targetPosition = (Vector2)aimed.Target.position;

                entity.AddComponent<Cooldown>().Current = shooter.Cooldown;
                entity.RemoveComponent<Aimed>();
                
                ref var request = ref World.CreateEntity().AddComponent<CreateProjectileRequest>();
                request.InitialPosition = shooterPosition;
                request.TargetPosition = targetPosition;
                request.StartBulletSpeedScale = startBulletSpeedScale;
            }
        }
    }
}