using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CollisionSystem))]
    public sealed class CollisionSystem : FixedUpdateSystem
    {
        private Filter _filter;
        private Stash<Colliding> _collidingStash;

        public override void OnAwake()
        {
            _filter = World.Filter.With<Colliding>().With<Health.Health>().Build();
            _collidingStash = World.GetStash<Colliding>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var colliding = _collidingStash.Get(entity);
                
                while (colliding.CollisionQueue.Count > 0)
                {
                    var collisionData = colliding.CollisionQueue.Dequeue();
                    var collisionEntity = World.CreateEntity();
                    collisionEntity.AddComponent<CollisionComponent>().Data = collisionData;
                    ProcessCollision(collisionData);
                }
            }
        }
        
        private void ProcessCollision(CollisionData data)
        {
            // Логика обработки столкновений внутри ECS (например, уменьшение здоровья)
            Debug.Log($"Collision detected between entities {data.Entity} and {data.OtherEntity}");
        }
    }
}