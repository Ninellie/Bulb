using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Aiming
{
    [Serializable]
    public struct Aiming : IComponent
    {
        [field: SerializeField] public float AimingRadius { get; set; }
        [field: SerializeField] public Transform Transform { get; set; }
    }
}