using System;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.Blocks
{
    [Serializable]
    public struct BlockName : IComponent
    {
        public string Name;
    }
}