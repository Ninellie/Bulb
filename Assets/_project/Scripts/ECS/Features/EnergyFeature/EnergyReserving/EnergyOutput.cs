using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.EnergyFeature.EnergyReserving
{
    /// <summary>
    /// Флаг, сочетающийся с Reserve, который указывает на то что Reserve может отдавать энергию. 
    /// </summary>
    [Serializable]
    public struct EnergyOutput : IComponent { }
}