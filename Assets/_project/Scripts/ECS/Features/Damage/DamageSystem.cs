using _project.Scripts.ECS.Features.Collisions;
using _project.Scripts.ECS.Features.Health;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Damage
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DamageSystem))]
    internal sealed class DamageSystem : FixedUpdateSystem
    {
        private Stash<CollisionComponent> _collisionStash;
        private Filter _collisionFilter;
        
        private Stash<DamageDealer> _damageDealerStash;
        private Stash<DamageTaker> _damageTakerStash;
        
        private Stash<HealthChangeRequest> _damageStash;

    
        public override void OnAwake()
        {
            _collisionFilter = World.Filter.With<CollisionComponent>().Build();
            _collisionStash = World.GetStash<CollisionComponent>();
            _damageDealerStash = World.GetStash<DamageDealer>();
            _damageTakerStash = World.GetStash<DamageTaker>();

            _damageStash = World.GetStash<HealthChangeRequest>();
        }
        
        public override void OnUpdate(float deltaTime)
        {
            foreach (var collision in _collisionFilter)
            {
                if (!_collisionStash.Has(collision))
                {
                    continue;
                }
                
                ref var collisionComponent = ref _collisionStash.Get(collision);
                var collisionData = collisionComponent.Data;
                var entity = collisionData.Entity;
                var otherEntity = collisionData.OtherEntity;

                var dealDamage = _damageDealerStash.Has(otherEntity); 
                
                if (dealDamage)
                {
                    var takeDamage = _damageTakerStash.Has(entity); 
                    
                    if (takeDamage)
                    {
                        var damageAmount = _damageDealerStash.Get(otherEntity).Amount;
                        var damageEntity = World.CreateEntity();
                        ref var damage = ref _damageStash.Add(damageEntity);
                        damage.TargetEntity = entity;
                        damage.Amount = -damageAmount; 
                    }
                }

                _collisionStash.Remove(collision);
            }
        }
    }
}