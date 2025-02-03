using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    public struct EnemyData : IComponent
    {
        public Transform Transform;
        public CircleCollider2D CircleCollider2D;
    }
}