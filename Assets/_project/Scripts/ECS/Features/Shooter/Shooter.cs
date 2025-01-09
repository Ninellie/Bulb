using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Shooter : IComponent
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] [Tooltip("Shoots per second")]  public FloatReference AttackSpeed { get; set; }
    }
}