using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Projectiles
{
    [Serializable]
    public struct CreateProjectileRequest : IComponent
    {
        [field: SerializeField] public Vector2 TargetPosition { get; set; }
        [field: SerializeField] public Vector2 InitialPosition { get; set; }
        [field: SerializeField] public float StartBulletSpeedScale { get; set; }
    }
}