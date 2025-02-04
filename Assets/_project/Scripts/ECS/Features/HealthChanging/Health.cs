using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.HealthChanging
{
    [Serializable]
    public struct Health : IComponent
    {
        public int HealthPoints;
    }
}