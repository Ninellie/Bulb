using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnergyConsumption
{
    [Serializable]
    public struct EnergyReserve : IComponent
    {
        [field: SerializeField] public float CurrentAmount { get; set; }
        [field: SerializeField] public float MaximumAmount { get; set; }
        // Удобно использовать для проставления единичной стоимости умения
        [field: SerializeField] public float SatisfactionAmount { get; set; } 
    }
}