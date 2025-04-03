using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.MovementSpeed
{
    /// <summary>
    /// Создаётся для удаления модификатора
    /// </summary>
    public struct MovementSpeedStatRemoveModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }
}