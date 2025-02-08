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
        private Filter _generatorsFilter;
        private Filter _nonFullConsumersFilter;
        
        public override void OnAwake()
        {
            _generatorsFilter = World.Filter.With<Generator>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            var amount = GetEnergyProductionAmount();
            
            if (amount > 0)
            {
                var eventEntity = World.CreateEntity();
                ref var generatedEvent = ref eventEntity.AddComponent<EnergyGeneratedEvent>();
                generatedEvent.Amount = amount;
            }
        }

        private float GetEnergyProductionAmount()
        {
            var amount = 0f;
            
            foreach (var entity in _generatorsFilter)
            {
                ref var generator = ref entity.GetComponent<Generator>();
                var productionAmount = generator.EnergyProductionAmount.Value;
                var baseCooldown = generator.BaseCooldown.Value;
                var productionRatePerSecond = productionAmount / baseCooldown;
                var producedEnergy = productionRatePerSecond * Time.fixedDeltaTime;
                amount = producedEnergy;
            }

            return amount;
        }
    }
}