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
                
                bool full;
                bool empty;
                var satisfied = false;
                
                if (container.CurrentAmount >= container.MaximumAmount)
                {
                    container.CurrentAmount = container.MaximumAmount;
                    full = true;
                    empty = false;
                }
                else
                {
                    full = false;
                }

                if (container.CurrentAmount <= 0)
                {
                    container.CurrentAmount = 0;
                    empty = true;
                    full = false;
                }
                else
                {
                    empty = false;
                }

                if (container.CurrentAmount >= container.SatisfactionAmount)
                {
                    satisfied = true;
                }

                if (full)
                {
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

                if (empty)
                {
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

                if (satisfied)
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
}