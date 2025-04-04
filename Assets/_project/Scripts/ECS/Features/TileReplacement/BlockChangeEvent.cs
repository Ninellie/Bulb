using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    /// <summary>
    /// Только для чтения.
    /// Компонент-событие создающееся после того как была совершена смена блока.
    /// Содержит позицию и сущность нового блока.
    /// Живёт один кадр, создаётся и удаляется исключительно в TileReplacementSystem. 
    /// </summary>
    public struct BlockChangeEvent : IComponent
    {
        public Vector3Int Position;
        public BlockData OldBlockData;
        public BlockData NewBlockData;
        public Entity NewEntity;
    }
}