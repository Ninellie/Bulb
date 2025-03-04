using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Moving
{
    [Serializable]
    public struct Movable : IComponent
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] public Vector2 Direction { get; set; }
        [field: SerializeField] public float SpeedScale {get; set;}
        [field: SerializeField] public float SpeedScaleChangePerSecond { get; set; }
    }
}