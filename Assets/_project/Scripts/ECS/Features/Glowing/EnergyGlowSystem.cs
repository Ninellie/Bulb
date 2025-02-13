using _project.Scripts.ECS.Features.EnergyReserving;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Glowing
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(EnergyGlowSystem))]
    public sealed class EnergyGlowSystem : UpdateSystem
    {
        private const float GlowingMultiplier = 35f; 
        
        private Filter _glowingEnergyContainersFilter;
        
        public override void OnAwake()
        {
            _glowingEnergyContainersFilter = World.Filter
                .With<Glowing>()
                .With<EnergyContainer>()
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _glowingEnergyContainersFilter)
            {
                ref var glowing = ref entity.GetComponent<Glowing>();
                ref var energyReserve = ref entity.GetComponent<EnergyContainer>();
                glowing.Light2D.pointLightOuterRadius = energyReserve.CurrentAmount * GlowingMultiplier;
            }
        }
    }
}