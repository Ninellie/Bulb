using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Health
{
    [Serializable]
    public struct HealthComponent : IComponent
    {
        [SerializeField] private int healthPoints;

        public int HealthPoints
        {
            get => healthPoints;
            set => healthPoints = value;
        }
    }
}