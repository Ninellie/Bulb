using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.EnergyFeature.EnergyReserving
{
    [Serializable]
    public struct EnergyContainer : IComponent
    {
        public float CurrentAmount;
        public float MaximumAmount;
        // Удобно использовать для проставления единичной стоимости умения
        public float SatisfactionAmount;
    }
}