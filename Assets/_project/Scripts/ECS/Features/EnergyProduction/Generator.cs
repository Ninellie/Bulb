using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyProduction
{
    [Serializable]
    public struct Generator : IComponent
    {
        //[field: SerializeField] [Tooltip("Energy production rate per second")]  public FloatReference EnergyProductionRate { get; set; }
        [field: SerializeField] public FloatReference EnergyProductionAmount { get; set; }
        [field: SerializeField] public FloatReference BaseCooldown { get; set; }
    }
}