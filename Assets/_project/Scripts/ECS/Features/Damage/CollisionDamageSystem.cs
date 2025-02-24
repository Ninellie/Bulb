using _project.Scripts.ECS.Features.Collisions;
using _project.Scripts.ECS.Features.HealthChanging;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Damage
{
    /// <summary>
    /// Содержит данные о коллизии двух сущностей, создаётся скриптом CollidingProvider
    /// </summary>
    public struct MakeDamageRequest : IComponent
    {
        public Entity Entity;
        public Entity OtherEntity;
        public int Amount;
    }
    
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CollisionDamageSystem))]
    internal sealed class CollisionDamageSystem : FixedUpdateSystem
    {
        private Stash<OnCollision2DEnterEvent> _collisionStash;
        
        private Stash<DamageDealer> _damageDealerStash;
        private Stash<DamageTaker> _damageTakerStash;
        
        private Stash<HealthChangeRequest> _healthChangeRequest;
        
        public override void OnAwake()
        {
            _collisionStash = World.GetStash<OnCollision2DEnterEvent>();
            
            _damageDealerStash = World.GetStash<DamageDealer>();
            _damageTakerStash = World.GetStash<DamageTaker>();
            
            _healthChangeRequest = World.GetStash<HealthChangeRequest>();
        }
        
        public override void OnUpdate(float deltaTime)
        {
            foreach (ref var collisionEvent in _collisionStash)
            {
                var collisionEntity = collisionEvent.Entity;
                var otherEntity = collisionEvent.OtherEntity;
                
                if (collisionEntity.IsNullOrDisposed()) continue;
                if (otherEntity.IsNullOrDisposed()) continue;
                
                var canDealDamage = _damageDealerStash.Has(otherEntity);

                if (!canDealDamage) continue;
                
                var canTakeDamage = _damageTakerStash.Has(collisionEntity);

                if (!canTakeDamage) continue;
                
                var damageAmount = _damageDealerStash.Get(otherEntity).Amount;
                var damageEntity = World.CreateEntity();
                
                ref var healthChangeRequest = ref _healthChangeRequest.Add(damageEntity);
                
                healthChangeRequest.TargetEntity = collisionEntity;
                healthChangeRequest.Amount = -damageAmount;
            }
        }
    }
}