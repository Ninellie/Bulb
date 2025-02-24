using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// Содержит данные о коллизии двух сущностей, создаётся скриптом CollidingProvider
    /// </summary>
    public struct OnCollision2DEnter : IComponent
    {
        public Entity Entity;
        public Entity OtherEntity;
        public Collision2D Collision2D;
    }
}