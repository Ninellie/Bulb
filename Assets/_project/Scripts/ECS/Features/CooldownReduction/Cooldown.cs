using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.CooldownReduction
{
    [Serializable]
    public struct Cooldown : IComponent
    {
        [field: SerializeField] public float Current { get; set; }
    }
}