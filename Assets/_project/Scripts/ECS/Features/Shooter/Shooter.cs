using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Shooter : IComponent
    {
        [field: SerializeField] public Transform Transform { get; set; }
    }
}