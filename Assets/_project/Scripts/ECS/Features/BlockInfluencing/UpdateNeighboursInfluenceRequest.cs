using _project.Scripts.ECS.Features.TileReplacement;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlockInfluencing
{
    public struct UpdateNeighboursInfluenceRequest : IComponent
    {
        public Vector3Int Position;
        public BlockData OldBlockData;
        public BlockData NewBlockData;
        public Entity NewEntity;
    }
}