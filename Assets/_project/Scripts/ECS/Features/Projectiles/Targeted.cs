using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Projectiles
{
    [Serializable]
    public struct Targeted : IComponent
    {
        [field: SerializeField] public Vector2 CurrentTargetPosition { get; set; }
    }
}