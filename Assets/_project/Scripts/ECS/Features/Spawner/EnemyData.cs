﻿using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Spawner
{
    [Serializable]
    public struct EnemyData : IComponent
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] public CircleCollider2D CircleCollider2D { get; set; }
    }
}