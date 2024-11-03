using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// Определяет столкновения и создаёт новые сущности с информацией о столкновениях.
    /// Каждый физический кадр находит все Colliding Объёкты и создаёт новые сущности
    /// с компонентом CollisionCompomonent информацией о столкновением.
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CollisionDetectionSystem))]
    internal sealed class CollisionDetectionSystem : FixedUpdateSystem
    {
        private Filter _filter;
        private Stash<Colliding> _collidingStash;

        public override void OnAwake()
        {
            _filter = World.Filter.With<Colliding>().With<Health.HealthComponent>().Build();
            _collidingStash = World.GetStash<Colliding>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var colliding = _collidingStash.Get(entity);

                if (colliding.CollisionQueue == null)
                {
                    continue;
                }
                
                while (colliding.CollisionQueue.Count > 0)
                {
                    var data = colliding.CollisionQueue.Dequeue();
                    var collisionEntity = World.CreateEntity();
                    collisionEntity.AddComponent<CollisionComponent>().Data = data;
                    Debug.Log($"Collision detected between entities {data.Entity} and {data.OtherEntity}");
                }
            }
        }
    }
}