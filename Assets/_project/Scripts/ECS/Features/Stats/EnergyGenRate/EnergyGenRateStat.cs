using System;
using System.Collections.Generic;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.EnergyGenRate
{
    /// <summary>
    /// Изменяется только внутри системы EnergyGenRateCalculatingSystem.
    /// Для изменения извне, другие системы могут вызывать EnergyGenRateAddModRequest и EnergyGenRateRemoveModRequest
    /// </summary>
    [Serializable]
    public struct EnergyGenRateStat : IComponent
    {
        public float Base;
        public float Current;
        public float Min;
        public float Max;
        public List<StatMod> Mods;
    }
}