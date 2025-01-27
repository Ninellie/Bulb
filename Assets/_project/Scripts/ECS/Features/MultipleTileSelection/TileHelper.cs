using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.MultipleTileSelection
{
    public static class TileHelper
    {
        public static HashSet<Vector3Int> GetTilesWithinBounds(Tilemap tilemap, Bounds bounds)
        {
            var tiles = new HashSet<Vector3Int>();

            // Преобразуем границы из мировых координат в координаты клеток Tilemap
            var minCell = tilemap.WorldToCell(bounds.min + tilemap.cellSize / 2);
            var maxCell = tilemap.WorldToCell(bounds.max + tilemap.cellSize / 2);
            
            // Перебираем все тайлы внутри прямоугольной области
            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    var cellPosition = new Vector3Int(x, y, 0);

                    // Проверяем, есть ли тайл в данной позиции
                    if (tilemap.HasTile(cellPosition))
                    {
                        tiles.Add(cellPosition);
                    }
                }
            }

            return tiles;
        }
    }
}