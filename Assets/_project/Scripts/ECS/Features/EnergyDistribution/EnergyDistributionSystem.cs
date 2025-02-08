using _project.Scripts.ECS.Features.EnergyProduction;
using _project.Scripts.ECS.Features.EnergyReserving;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyDistribution
{
    /// <summary>
    /// Если есть потребность, равномерно извлекает энергию у аккумуляторов
    /// и равномерно распределяет её по потребителям 
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyDistributionSystem))]
    public sealed class EnergyDistributionSystem : FixedUpdateSystem
    {
        private Filter _nonEmptyDistributorFilter;
        private Filter _nonFullReceiversFilter;
        
        private Filter _nonFullConsumersFilter;
        
        private Stash<EnergyGeneratedEvent> _energyGeneratedEventStash;
        public override void OnAwake()
        {
            _nonEmptyDistributorFilter = World.Filter // Любой непустой отдающий контейнер
                .With<EnergyContainer>()
                .With<EnergyOutput>()
                .Without<EnergyEmpty>()
                .Build();
            
            _nonFullReceiversFilter = World.Filter // Любой неполный поглощающий контейнер
                .With<EnergyContainer>()
                .With<EnergyInput>()
                .Without<EnergyFull>()
                .Build();
             
            _nonFullConsumersFilter = World.Filter // Неполный только поглощающий контейнер
                .With<EnergyContainer>()
                .With<EnergyInput>()
                .Without<EnergyOutput>()
                .Without<EnergyFull>()
                .Build();

            _energyGeneratedEventStash = World.GetStash<EnergyGeneratedEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            var generatedAmount = 0f;
            
            foreach (ref var generatedEvent in _energyGeneratedEventStash)
            {
                generatedAmount += generatedEvent.Amount;
            }
            
            DistributeEnergy(generatedAmount, _nonFullReceiversFilter);
            
            // Определить нужное количество
            var needed = GetNeededAmount();
            
            // Определить сколько есть всего
            var contained = GetAvailableAmount();
            
            // Определить сколько снимать
            var toWithdraw = Mathf.Min(needed, contained);
            
            // Изъять количество
            var rest = WithdrawEnergy(toWithdraw, _nonEmptyDistributorFilter);
            
            toWithdraw -= rest;
            
            // Распределить
            DistributeEnergy(toWithdraw, _nonFullConsumersFilter);
        }
        
        /// <summary>
        /// Равномерно распределяет энергию по всем контейнерам энергии которые могут принимать её
        /// </summary>
        /// <param name="energyAmount"></param>
        /// <param name="filter"></param>
        /// <returns>Остаток энергии, которую не удалось распределить</returns>
        private float DistributeEnergy(float energyAmount, Filter filter)
        {
            if (energyAmount <= 0f)
            {
                return 0f;
            }
            
            var remainingAmount = energyAmount;
            
            while (remainingAmount > 0)
            {
                var consumersCount = 0;
                    
                foreach (var entity in filter)
                {
                    consumersCount++;
                }
                
                if (consumersCount <= 0)
                {
                    return remainingAmount;
                }
                
                var share = energyAmount / consumersCount;
                
                foreach (var entity in filter)
                {
                    ref var container = ref entity.GetComponent<EnergyContainer>();
                    
                    var spaceAvailable = container.MaximumAmount - container.CurrentAmount;
                    
                    if (spaceAvailable <= 0)
                    {
                        entity.AddComponent<EnergyFull>();
                        continue;
                    }
                    
                    var toAdd = Mathf.Min(spaceAvailable, share);
                    
                    container.CurrentAmount += toAdd;
                    
                    remainingAmount -= toAdd;

                    if (container.CurrentAmount >= container.MaximumAmount)
                    { 
                        container.CurrentAmount = container.MaximumAmount;
                        entity.AddComponent<EnergyFull>();
                    }

                    entity.RemoveComponent<EnergyEmpty>();
                }
                
                World.Commit();
            }
            
            return remainingAmount;
        }
        
        private float WithdrawEnergy(float energyAmount, Filter filter)
        {
            if (energyAmount <= 0f)
            {
                return 0f;
            }
            
            var remainingAmount = energyAmount;
            
            while (remainingAmount > 0)
            {
                var consumersCount = 0;
                
                foreach (var entity in filter)
                {
                    consumersCount++;
                }
                
                if (consumersCount <= 0)
                {
                    return remainingAmount;
                }
                
                var share = energyAmount / consumersCount;
                
                foreach (var entity in filter)
                {
                    ref var container = ref entity.GetComponent<EnergyContainer>();
                    
                    if (container.CurrentAmount <= 0)
                    {
                        entity.AddComponent<EnergyEmpty>();
                        continue;
                    }
                    
                    var toWithdraw = Mathf.Min(container.CurrentAmount, share);
                    
                    container.CurrentAmount -= toWithdraw;
                    
                    remainingAmount -= toWithdraw;

                    if (container.CurrentAmount <= 0)
                    { 
                        container.CurrentAmount = 0;
                        entity.AddComponent<EnergyEmpty>();
                    }

                    entity.RemoveComponent<EnergyFull>();
                }
                
                World.Commit();
            }
            
            return remainingAmount;
        }
        
        private float GetNeededAmount()
        {
            return GetAmount(_nonFullConsumersFilter);
        }

        private float GetAvailableAmount()
        {
            // TODO Добавить EnergyInput компоненту скорость отдачи и учитывать его в этой функции
            return GetAmount(_nonEmptyDistributorFilter);
        }

        private static float GetAmount(Filter filter)
        {
            var amount = 0f;
            
            foreach (var entity in filter)
            {
                var container = entity.GetComponent<EnergyContainer>();
                amount += container.CurrentAmount;
            }
            
            return amount;
        }
    }
}