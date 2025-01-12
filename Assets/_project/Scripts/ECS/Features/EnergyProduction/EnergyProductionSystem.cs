using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.CooldownReduction;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyProduction
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyProductionSystem))]
    public sealed class EnergyProductionSystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable currentEnergy;
        [SerializeField] private FloatVariable energyMaximum;

        private Filter _generatorsFilter;
        
        public override void OnAwake()
        {
            _generatorsFilter = World.Filter.With<Generator>()
                .Without<Disabled>()
                .Without<Cooldown>()
                .Build();
            
            foreach (var entity in _generatorsFilter)
            {
                ref var generator = ref entity.GetComponent<Generator>();
                var baseCooldown = generator.BaseCooldown.Value;
                entity.AddComponent<Cooldown>().Current = baseCooldown;
            }
        }

        public override void OnUpdate(float deltaTime)
        {
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
    }
}