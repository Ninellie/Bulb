using System;
    using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    [Serializable]
    public class BlockData
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public TileBase Prefab { get; set; }
        [field: SerializeField] public int Cost { get; set; }
    }
}