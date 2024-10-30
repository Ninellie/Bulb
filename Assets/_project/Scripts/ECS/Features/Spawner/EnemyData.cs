using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    [Serializable]
    public struct EnemyData : IComponent
    {
        [SerializeField] private Transform transform;
        [SerializeField] private Rigidbody2D rigidbody2D;
        
        public Transform Transform => transform;
        public Rigidbody2D Rigidbody2D => rigidbody2D;
    }
}