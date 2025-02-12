﻿using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Shooter : IComponent, IAbility
    {
        [field: SerializeField] public Transform Transform { get; set; }
        [field: SerializeField] [Tooltip("Delay after shoot")]  public float Cooldown { get; set; }
        [field: SerializeField] [Tooltip("Energy cost per use")] public float Cost { get; set; }
        
        [field: SerializeField] [Tooltip("Penalty time of disability due to lack of energy.")]
        public float PenaltyTime { get; set; }
    }
}