using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    /// <summary>
    /// Компонент-событие. Живёт полный кадр, после чего убивается системой столкновений.
    /// </summary>
    public struct OnCollision2DEnterEvent : IComponent
    {
        public Entity Entity;
        public Entity OtherEntity;
        public Collision2D Collision2D;
    }
}