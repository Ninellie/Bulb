using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.EnergyGenRate
{
    /// <summary>
    /// Создаётся для добавления модификатора
    /// </summary>
    public struct EnergyGenRateStatAddModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }
}