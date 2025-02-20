using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.CooldownReduction;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyFeature.EnergyProduction
{
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyProductionSystem))]
    public sealed class EnergyProductionSystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable currentEnergy;
        [SerializeField] private FloatVariable energyMaximum;
        [SerializeField] private FloatVariable producedTotal;
        [SerializeField] private FloatVariable productionRate;

        private const float StartEnergy = 0f;
        
        private Filter _generatorsFilter;
        
        public override void OnAwake()
        {
            currentEnergy.SetValue(StartEnergy);
            
            _generatorsFilter = World.Filter
                .With<Generator>()
                .Without<Cooldown>()
                .Build();
            
            SetupBaseCooldown();
        }

        public override void OnUpdate(float deltaTime)
        {
            CalculateProduction();
            
            foreach (var entity in _generatorsFilter)
            {
                ref var generator = ref entity.GetComponent<Generator>();
                var producedEnergy = generator.EnergyProductionAmount.Value;
                var nextValue = currentEnergy.value + producedEnergy;
                currentEnergy.SetValue(!(nextValue > energyMaximum.value) ? nextValue : energyMaximum.value);
                var baseCooldown = generator.BaseCooldown.Value;
                entity.AddComponent<Cooldown>().Current = baseCooldown;
            }
        }

        private void CalculateProduction()
        {
            foreach (var entity in _generatorsFilter)
            {
                ref var generator = ref entity.GetComponent<Generator>();
                var producedEnergy = generator.EnergyProductionAmount.Value;
                producedTotal.ApplyChange(producedEnergy);
            }
            
            var total = producedTotal.value;
            productionRate.SetValue(total / Time.timeSinceLevelLoad);
        }

        private void SetupBaseCooldown()
        {
            foreach (var entity in _generatorsFilter)
            {
                ref var generator = ref entity.GetComponent<Generator>();
                
                // Или так или выставить rate per second и умножать значение в секунду,
                // но тогда нельзя делать разные смешные типы генераторов,
                // которые производят энергию раз в определённый период.
                var baseCooldown = generator.BaseCooldown.Value;
                entity.AddComponent<Cooldown>().Current = baseCooldown;
            }
        }
    }
}