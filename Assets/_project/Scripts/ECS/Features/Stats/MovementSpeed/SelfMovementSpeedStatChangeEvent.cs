using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.MovementSpeed
{
    /// <summary>
    /// Создаётся при изменении значения стата MovementSpeed или его компонентов
    /// </summary>
    public struct SelfMovementSpeedStatChangeEvent : IComponent
    {
        public float Value;
    }
}