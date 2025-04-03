using _project.Scripts.ECS.Features.Stats.EnergyGenRate;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyFeature.EnergyProduction
{
    /// <summary>
    /// Извлекает энергию из генераторов и создаёт событие
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyGenerationSystem))]
    public sealed class EnergyGenerationSystem : FixedUpdateSystem
    {
        private Filter _generators;
        
        private Stash<EnergyGeneratedEvent> _energyGeneratedEventsStash;
        
        public override void OnAwake()
        {
            _generators = World.Filter
                .With<Generator>()
                .With<EnergyGenRateStat>()
                .Build();
            
            _energyGeneratedEventsStash = World.GetStash<EnergyGeneratedEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _energyGeneratedEventsStash.RemoveAll();
            
            var amount = GetEnergyGenerationAmountPerSecond();
             amount *= deltaTime;
             
            if (amount > 0)
            {
                var eventEntity = World.CreateEntity();
                ref var generatedEvent = ref _energyGeneratedEventsStash.Add(eventEntity);
                generatedEvent.Amount = amount;
            }
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