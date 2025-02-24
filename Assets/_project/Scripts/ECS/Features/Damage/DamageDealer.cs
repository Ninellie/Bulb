using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Damage
{
    [Serializable]
    public struct DamageDealer : IComponent
    {
        public int Amount;
    }
}