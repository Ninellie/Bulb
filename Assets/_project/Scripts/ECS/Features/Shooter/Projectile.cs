using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Projectile : IComponent
    {
        [SerializeField] private Transform transform;
        
        public Transform Transform { get => transform; set => transform = value; }
    }
}