using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [Serializable]
    public struct Movable : IComponent
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] public Vector2 Direction { get; set; }
        [field: SerializeField] public FloatReference Speed { get; set; }
        [field: SerializeField] public float SpeedScale {get; set;}
        [field: SerializeField] public float SpeedScaleChangePerSecond { get; set; }
    }
}