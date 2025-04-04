using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlockNeighboursSetting
{
    /// <summary>
    /// Класс обозначает блок-сосед к другому блоку.
    /// Хранит Entity соседа и относительную позицию к другому блоку.
    /// </summary>
    [Serializable]
    public class Neighbor
    {
        public Entity Entity;
        public Vector3Int RelativePosition;
    }
}