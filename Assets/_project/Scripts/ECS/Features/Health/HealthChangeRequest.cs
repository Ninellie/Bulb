using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Health
{
    [Serializable]
    public struct HealthChangeRequest : IComponent
    {
        public Entity TargetEntity { get; set; }
        public int Amount { get; set; }
    }
}