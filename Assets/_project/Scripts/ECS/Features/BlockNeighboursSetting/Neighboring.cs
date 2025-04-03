using System.Collections.Generic;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.BlockNeighboursSetting
{
    public struct Neighboring : IComponent
    {
        public List<Neighbor> Neighbors;
    }
}