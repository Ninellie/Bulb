using System;
using _project.Scripts.ECS.Features.Blocks;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.BlockNeighboursSetting
{
    public static class NeighboursUtils
    {
        public enum Direction
        {
            Top,
            RightTop,
            Right,
            RightBottom,
            Bottom,
            LeftBottom,
            Left,
            LeftTop
        }
        
        public static Neighbor GetNeighbour(Tilemap tilemap, Vector3Int position, Direction direction)
        {
            var dirVector = GetDirection(direction);
            var go = tilemap.GetInstantiatedObject(position + dirVector);
            var provider = go.GetComponent<BlockNameProvider>(); 
            var entity = provider.Entity; 
            var neighbour = new Neighbor
            {
                RelativePosition = dirVector,
                Entity = entity
            };
            return neighbour;
        }

        private static Vector3Int GetDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Top => new Vector3Int(0, 1),
                Direction.RightTop => new Vector3Int(1, 1),
                Direction.Right => new Vector3Int(1, 0),
                Direction.RightBottom => new Vector3Int(1, -1),
                Direction.Bottom => new Vector3Int(0, -1),
                Direction.LeftBottom => new Vector3Int(-1, -1),
                Direction.Left => new Vector3Int(-1, 0),
                Direction.LeftTop => new Vector3Int(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
    }
}