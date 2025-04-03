using System;
using System.Collections.Generic;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Stats.MovementSpeed
{
    /// <summary>
    /// Изменяется только внутри системы MovementSpeedStatCalculatingSystem.
    /// Для изменения извне, другие системы могут вызывать MovementSpeedStatAddModRequest и MovementSpeedStatRemoveModRequest
    /// </summary>
    [Serializable]
    public struct MovementSpeedStat : IComponent
    {
        public float Base;
        public float Current;
        public float Min;
        public float Max;
        public List<StatMod> Mods;
    }
}