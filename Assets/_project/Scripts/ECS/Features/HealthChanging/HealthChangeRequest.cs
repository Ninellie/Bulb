using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.HealthChanging
{
    [Serializable]
    public struct HealthChangeRequest : IComponent
    {
        public Entity TargetEntity;
        public int Amount;
    }
}