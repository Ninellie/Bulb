using System;
using _project.Scripts.ECS.Features.Moving;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Projectiles
{
    [Serializable]
    public struct Projectile : IComponent
    {
        [field: SerializeField] public MovableProvider MovableProvider { get; set; }
    }
}