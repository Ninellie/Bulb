using System;
using _project.Scripts.Core.Variables;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyConsumption
{
    [Serializable]
    public enum EnergyDistributionMode
    {
        Equally,
        OneByOne
    }
    
    /// <summary>
    /// Система потребления энергию распределяет общую энергию по потребителям.
    /// Но потребители сами как-то тратят энергию. Поэтому система также проверяет статус резерва потребителей.
    /// 
    /// Пытается сначала удовлетворить потребителей с пустыми резервами
    /// Потом заполнить удовлетворённых.
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyConsumptionSystem))]
    public sealed class EnergyConsumptionSystem : FixedUpdateSystem
    {
        [SerializeField] private EnergyDistributionMode distributionMode = EnergyDistributionMode.Equally;
        [SerializeField] private FloatVariable currentEnergy;
        
        // Только для установки значения, для инфы
        [SerializeField] private FloatVariable consumedTotal;
        [SerializeField] private FloatVariable consumptionRate;
        
        // Сначала это
        private Filter _nonSatisfiedConsumersFilter;
        // Потом это
        private Filter _nonFullSatisfiedConsumersFilter;
        
        private Filter _fullConsumersFilter;
        
        private Filter _nonFullConsumersFilter;
        
        // todo решение довольно плохое, так как большие потребители будут всегда выжирать всю энергию.
        // Нужно чтобы после удовлетворения энергия распределялась небольшими порциями. Возможно с мануальным выставлением мода.
        
        public override void OnAwake()
        {
            _nonSatisfiedConsumersFilter = World.Filter
                .With<EnergyReserve>()
                .Without<EnergySatisfied>()
                .Build();
            
            _nonFullSatisfiedConsumersFilter = World.Filter
                .With<EnergyReserve>()
                .With<EnergySatisfied>()
                .Without<EnergyFull>()
                .Build();

            _nonFullConsumersFilter = World.Filter
                .With<EnergyReserve>()
                .Without<EnergyFull>()
                .Build();
            
            _fullConsumersFilter = World.Filter
                .With<EnergyReserve>()
                .With<EnergyFull>()
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            // удостовериться что они всё ещё удовлетворены
            VerifyConsumersStatus();
            // todo на подумать: penalty time возможно не должно быть статичным временем, но вместо этого ждать пока резерв не заполнится полностью 

            switch (distributionMode)
            {
                case EnergyDistributionMode.Equally:
                    FillConsumersEqually();
                    break;
                case EnergyDistributionMode.OneByOne:
                    SatisfyConsumersOneByOne();
                    FillConsumersOneByOne();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
#if UNITY_EDITOR
            consumptionRate.SetValue(consumedTotal.value / Time.timeSinceLevelLoad);
#endif
        }

        private void VerifyConsumersStatus()
        {
            foreach (var entity in _nonFullSatisfiedConsumersFilter)
            {
                ref var energyReserve = ref entity.GetComponent<EnergyReserve>();
                if (!(energyReserve.CurrentAmount < energyReserve.SatisfactionAmount)) continue;
                entity.RemoveComponent<EnergySatisfied>();
            }
            
            foreach (var entity in _fullConsumersFilter)
            {
                ref var energyReserve = ref entity.GetComponent<EnergyReserve>();
                if (!(energyReserve.CurrentAmount < energyReserve.MaximumAmount)) continue;
                entity.RemoveComponent<EnergyFull>();
            }
        }

        private void SatisfyConsumersOneByOne()
        {
            foreach (var entity in _nonSatisfiedConsumersFilter)
            {
                ref var reserve = ref entity.GetComponent<EnergyReserve>();
                var needToSatisfy = reserve.SatisfactionAmount - reserve.CurrentAmount;

                if (currentEnergy.value - needToSatisfy <= 0) continue;
                
                consumedTotal.ApplyChange(needToSatisfy);
                
                currentEnergy.ApplyChange(-needToSatisfy);
                reserve.CurrentAmount = reserve.SatisfactionAmount;
                entity.AddComponent<EnergySatisfied>();
                entity.RemoveComponent<EnergyEmpty>();
            }
        }
        
        private void FillConsumersOneByOne()
        {
            foreach (var entity in _nonFullSatisfiedConsumersFilter)
            {
                ref var reserve = ref entity.GetComponent<EnergyReserve>();
                var needToFill = reserve.MaximumAmount - reserve.CurrentAmount;
                
                if (currentEnergy.value - needToFill <= 0) continue;
                
                consumedTotal.ApplyChange(needToFill);
                
                currentEnergy.ApplyChange(-needToFill);
                reserve.CurrentAmount = reserve.MaximumAmount;
                entity.AddComponent<EnergyFull>();
                entity.RemoveComponent<EnergyEmpty>();
            }
        }
        
        private void FillConsumersEqually()
        {
            var nonFullConsumers = 0;
            
            foreach (var entity in _nonFullConsumersFilter)
            {
                nonFullConsumers++; 
            }
            
            var energyPortion = currentEnergy.value / nonFullConsumers;
            
            foreach (var entity in _nonFullConsumersFilter)
            {
                ref var reserve = ref entity.GetComponent<EnergyReserve>();
                var needToFill = reserve.MaximumAmount - reserve.CurrentAmount;
                
                // Если порция больше чем нужно для заполнения (после этого действия энергия станет полной)
                if (energyPortion > needToFill)
                {
                    entity.AddComponent<EnergyFull>();
                    reserve.CurrentAmount = reserve.MaximumAmount;
                    currentEnergy.ApplyChange(-needToFill);
                    consumedTotal.ApplyChange(needToFill);
                }
                else
                {
                    reserve.CurrentAmount += energyPortion;
                    currentEnergy.ApplyChange(-energyPortion);
                    consumedTotal.ApplyChange(energyPortion);
                }
                
                entity.RemoveComponent<EnergyEmpty>();
            }
        }
    }


    /*
    /// <summary>
    /// Заполняется полностью если хватает энергии. Главный приоритет.
    /// </summary>
    [Serializable]
    public struct PrimaryEnergyConsumer : IComponent { }
    
    /// <summary>
    /// Заполняется до удовлетворения, если хватает энергии. Главный приоритет.
    /// </summary>
    [Serializable]
    public struct SatisfiedEnergyConsumer : IComponent { }
    
    /// <summary>
    /// Энергия поровну распределяется между всеми параллельными потребителями. Вторичный приоритет.
    /// </summary>
    [Serializable]
    public struct ParallelEnergyConsumer : IComponent { }
    
    /// <summary>
    /// Заполняется только если все остальные потребители полностью заполнены.
    /// </summary>
    [Serializable]
    public struct OptionalEnergyConsumer : IComponent { }
    */
}