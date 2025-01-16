using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [Serializable]
    public struct Targeted : IComponent
    {
        [field: SerializeField] public Vector2Reference TargetPosition { get; set; }
    }
}