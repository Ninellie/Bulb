using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyProduction
{
    /// <summary>
    /// Извлекает энергию из генераторов и создаёт событие
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyGenerationSystem))]
    public sealed class EnergyGenerationSystem : FixedUpdateSystem
    {
        private Stash<Generator> _generatorsStash;
        private Stash<EnergyGeneratedEvent> _energyGeneratedEventsStash;
        
        public override void OnAwake()
        {
            _generatorsStash = World.GetStash<Generator>();
            _energyGeneratedEventsStash = World.GetStash<EnergyGeneratedEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _energyGeneratedEventsStash.RemoveAll();
            
            var amount = GetGeneratedEnergyAmount();
            
            if (amount > 0)
            {
                var eventEntity = World.CreateEntity();
                ref var generatedEvent = ref _energyGeneratedEventsStash.Add(eventEntity);
                generatedEvent.Amount = amount;
            }
        }

        private float GetGeneratedEnergyAmount()
        {
            var productionRatePerSecond = 0f;
            
            foreach (ref var generator in _generatorsStash)
            {
                var productionAmount = generator.EnergyProductionAmount.Value;
                var baseCooldown = generator.BaseCooldown.Value;
                productionRatePerSecond += productionAmount / baseCooldown;
            }

            return productionRatePerSecond * Time.fixedDeltaTime;
        }
    }
}