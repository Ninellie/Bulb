using System;
using System.Collections.Generic;
using _project.Scripts.ECS.Features.BlockInfluencing;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    [Serializable]
    public class BlockData
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public TileBase TileBasePrefab { get; set; }
        [field: SerializeField] public int Cost { get; set; }
        [field: SerializeField] public Sprite Picture { get; set; }
        [field: SerializeField] public List<BlockEffect> Effects { get; set; }
    }
}