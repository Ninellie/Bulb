using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Collisions
{
    [Serializable]
    public struct CollisionData
    {
        public Entity Entity { get; set; }
        public Entity OtherEntity { get; set; } 
        public Vector3 CollisionPoint { get; set; }
    }
}