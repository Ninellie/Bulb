using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    public struct TilesChangeRequest : IComponent
    {
        public List<TileChangeData> TileChangeData;
    }
}