using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// Определяет столкновения и создаёт новые сущности-ивенты с информацией о столкновениях.
    /// Каждый физический кадр находит все Colliding Объёкты и создаёт новые сущности
    /// с компонентом Collision информацией о столкновении.
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(Collision2DSystem))]
    internal sealed class Collision2DSystem : FixedUpdateSystem
    {
        private Stash<OnCollision2DEnter> _onCollision2DEnterStash;
        private Stash<OnCollision2DEnterEvent> _onCollision2DEnterEventStash;
        
        public override void OnAwake()
        {
            _onCollision2DEnterStash = World.GetStash<OnCollision2DEnter>();
            _onCollision2DEnterEventStash = World.GetStash<OnCollision2DEnterEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _onCollision2DEnterEventStash.RemoveAll();
            
            foreach (ref var collision in _onCollision2DEnterStash)
            {
                var entity = World.CreateEntity();
                ref var collisionEvent = ref _onCollision2DEnterEventStash.Add(entity);
                collisionEvent.Entity = collision.Entity;
                collisionEvent.OtherEntity = collision.OtherEntity;
                collisionEvent.Collision2D = collision.Collision2D;
            }
            
            _onCollision2DEnterStash.RemoveAll();
        }
    }
}