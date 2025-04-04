using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyProduction;
using _project.Scripts.ECS.Features.EnergyFeature.EnergyReserving;
using _project.Scripts.ECS.Features.Stats.EnergyGenRate;
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
        private Filter _generators;
        
        public override void OnAwake()
        {
            _generators = World.Filter
                .With<Generator>()
                .With<EnergyGenRateStat>()
                .Build();
            
            _accumulatorFilter = World.Filter
                .With<EnergyContainer>()
                .With<EnergyOutput>()
                .With<EnergyInput>()
                .Build();
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

            var genRate = GetEnergyGenerationAmountPerSecond();
            generationRate.SetValue(genRate);
        }
        
        private float GetEnergyGenerationAmountPerSecond()
        {
            var productionRatePerSecond = 0f;
            foreach (var generator in _generators)
            {
                productionRatePerSecond += generator.GetComponent<EnergyGenRateStat>().Current;
            }
            return productionRatePerSecond;
        }
    }
}