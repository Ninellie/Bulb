using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.EnergyGenRate
{
    /// <summary>
    /// Создаётся для удаления модификатора
    /// </summary>
    public struct EnergyGenRateStatRemoveModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }
}