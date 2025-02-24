using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Shooter
{
    [Serializable]
    public struct Shooter : IComponent
    {
        public Transform Transform;

        [Tooltip("Delay after shoot")]
        public float Cooldown;

        [Tooltip("Energy cost per use")]
        public float Cost;

        [Tooltip("Penalty time of disability due to lack of energy.")]
        public float PenaltyTime;
    }
}