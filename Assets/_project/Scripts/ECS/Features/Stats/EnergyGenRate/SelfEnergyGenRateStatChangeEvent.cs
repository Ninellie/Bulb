using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.EnergyGenRate
{
    /// <summary>
    /// Создаётся при изменении значения стата Template или его компонентов
    /// </summary>
    public struct SelfEnergyGenRateStatChangeEvent : IComponent
    {
        public float Value;
    }
}