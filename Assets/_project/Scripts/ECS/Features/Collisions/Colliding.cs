using System;
using System.Collections.Generic;
using Scellecs.Morpeh;
using Unity.IL2CPP.CompilerServices;

namespace _project.Scripts.ECS.Features.Collisions
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]

    [Serializable]
    public struct Colliding : IComponent
    {
        public Queue<CollisionData> CollisionQueue { get; set; }
    }
}