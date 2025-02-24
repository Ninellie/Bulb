using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// При столкновении двух Unity GameObject с таким же компонентом,
    /// создаёт сущность с компонентом столкновения для обработки системой столкновений
    /// </summary>
    public sealed class Colliding2DProvider : MonoProvider<Colliding2D>
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.TryGetComponent(out Colliding2DProvider provider)) return;
            
            var otherEntity = provider.Entity;

            if (otherEntity == null) return;

            var entity = World.Default.CreateEntity();
            
            ref var collisionComponent = ref entity.AddComponent<OnCollision2DEnter>();
            
            collisionComponent.Entity = Entity;
            collisionComponent.OtherEntity = otherEntity;
            collisionComponent.Collision2D = collision;
        }
    }
}