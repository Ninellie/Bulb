using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.MovementSpeed
{
    /// <summary>
    /// Создаётся для добавления модификатора
    /// </summary>
    public struct MovementSpeedStatAddModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }
}