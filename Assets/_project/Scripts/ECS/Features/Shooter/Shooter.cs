using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Shooter : IComponent, IAbility
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] [Tooltip("Delay after shoot")]  public FloatReference Cooldown { get; set; }
        [field: SerializeField] [Tooltip("Energy cost per use")] public FloatReference Cost { get; set; }
        
        [field: SerializeField] [Tooltip("Penalty time of disability due to lack of energy.")]
        public float PenaltyTime { get; set; }
    }
}