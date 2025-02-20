using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyProduction;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyReserving;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyFeature.EnergyStatistics
{
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyStatisticsSystem))]
    public sealed class EnergyStatisticsSystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable currentEnergy;
        [SerializeField] private FloatVariable maximumEnergy;
        [SerializeField] private FloatVariable generationRate;

        private Filter _accumulatorFilter;
        
        private Stash<Generator> _generatorsStash;
        
        public override void OnAwake()
        {
            _accumulatorFilter = World.Filter
                .With<EnergyContainer>()
                .With<EnergyOutput>()
                .With<EnergyInput>()
                .Build();
            
            _generatorsStash = World.GetStash<Generator>();
        }

        public override void OnUpdate(float deltaTime)
        {
            var energyCurrent = 0f;
            var energyMax = 0f;
            
            foreach (var entity in _accumulatorFilter)
            {
                ref var energyContainer = ref entity.GetComponent<EnergyContainer>();
                energyCurrent += energyContainer.CurrentAmount;
                energyMax += energyContainer.MaximumAmount;
            }
            
            currentEnergy.SetValue(energyCurrent);
            maximumEnergy.SetValue(energyMax);

            var genRate = GetGenerationRate();
            generationRate.SetValue(genRate);
        }
        
        private float GetGenerationRate()
        {
            var generationRatePerSecond = 0f;
            
            foreach (ref var generator in _generatorsStash)
            {
                var productionAmount = generator.EnergyProductionAmount.Value;
                var baseCooldown = generator.BaseCooldown.Value;
                generationRatePerSecond += productionAmount / baseCooldown;
            }

            return generationRatePerSecond;
        }
    }
}