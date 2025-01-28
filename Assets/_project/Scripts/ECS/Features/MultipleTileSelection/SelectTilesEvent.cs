using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.MultipleTileSelection
{
    /// <summary>
    /// Однокадровый компонент-событие, предназначенный для чтения другими системами.
    /// Создаётся и удаляется исключительно в MultipleTileSelectionSystem.
    /// </summary>
    public struct SelectTilesEvent : IComponent
    {
        public HashSet<Vector3Int> SelectionPositions { get; set; }
    }
}