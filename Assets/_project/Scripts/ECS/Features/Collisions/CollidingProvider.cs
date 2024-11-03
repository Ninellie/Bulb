using System.Collections.Generic;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// При столкновении двух Unity GameObject с таким же компонентом,
    /// добавляет во внутреннее хранилище CollisionData о столкновении с собой.
    /// </summary>
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public sealed class CollidingProvider : MonoProvider<Colliding>
    {
        //Накапливает внури себя данные о коллизиях 
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"Detected collision {collision}");
            if (!collision.collider.TryGetComponent(out CollidingProvider provider)) return;
            
            var otherEntity = provider.Entity;

            if (otherEntity == null) return;
            var data = new CollisionData
            {
                Entity = Entity,
                OtherEntity = otherEntity,
                CollisionPoint = collision.contacts[0].point
            };
            ref var colliding = ref Entity.GetComponent<Colliding>();
            colliding.CollisionQueue ??= new Queue<CollisionData>();
            colliding.CollisionQueue.Enqueue(data);
        }
    }
}