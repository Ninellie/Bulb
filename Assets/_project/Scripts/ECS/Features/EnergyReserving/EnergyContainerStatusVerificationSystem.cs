using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyReserving
{
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(EnergyContainerStatusVerificationSystem))]
    public sealed class EnergyContainerStatusVerificationSystem : FixedUpdateSystem
    {
        private Filter _containersFilter;
        
        private Stash<EnergyEmpty> _emptyStash;
        private Stash<EnergyFull> _fullStash;
        private Stash<EnergySatisfied> _satisfiedStash;
        
        public override void OnAwake()
        {
            _containersFilter = World.Filter.With<EnergyContainer>().Build();
            
            _emptyStash = World.GetStash<EnergyEmpty>();
            _fullStash = World.GetStash<EnergyFull>();
            _satisfiedStash = World.GetStash<EnergySatisfied>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _containersFilter)
            {
                ref var container = ref entity.GetComponent<EnergyContainer>();

                var isFull = container.CurrentAmount >= container.MaximumAmount;
                var isEmpty = container.CurrentAmount <= 0;
                var isSatisfied = container.CurrentAmount >= container.SatisfactionAmount;

                SetFull(entity, isFull, ref container);
                SetEmpty(entity, isEmpty, ref container);
                SetSatisfied(entity, isSatisfied);
            }
        }

        private void SetFull(Entity entity, bool isFull, ref EnergyContainer container)
        {
            if (isFull)
            {
                container.CurrentAmount = container.MaximumAmount;
                if (!_fullStash.Has(entity))
                {
                    _fullStash.Add(entity);
                }
            }
            else
            {
                if (_fullStash.Has(entity))
                {
                    _fullStash.Remove(entity);
                }
            }
        }

        private void SetEmpty(Entity entity, bool isEmpty, ref EnergyContainer container)
        {
            if (isEmpty)
            {
                container.CurrentAmount = 0;
                if (!_emptyStash.Has(entity))
                {
                    _emptyStash.Add(entity);
                }
            }
            else
            {
                if (_emptyStash.Has(entity))
                {
                    _emptyStash.Remove(entity);
                }
            }
        }

        private void SetSatisfied(Entity entity, bool isSatisfied)
        {
            if (isSatisfied)
            {
                if (!_satisfiedStash.Has(entity))
                {
                    _satisfiedStash.Add(entity);   
                }
            }
            else
            {
                if (_satisfiedStash.Has(entity))
                {
                    _satisfiedStash.Remove(entity);   
                }
            }
        }
    }
}