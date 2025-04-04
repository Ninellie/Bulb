using System.Collections.Generic;
using Scellecs.Morpeh;

namespace _project.Scripts.ECS.Features.BlockNeighboursSetting
{
    /// <summary>
    /// Блок отслеживающий своих соседей. Соседи также должны иметь такой компонент.
    /// </summary>
    public struct Neighboring : IComponent
    {
        public List<Neighbor> Neighbors;
    }
}