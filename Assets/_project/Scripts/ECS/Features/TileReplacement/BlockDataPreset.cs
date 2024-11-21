using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    public class BlockDataPreset : ScriptableObject
    {
        [SerializeField] private List<BlockData> blockDataList;
        [field: SerializeField] [ReadOnly] public BlockData Current { get; private set; }
        
        private void OnValidate()
        {
            foreach (var blockData in blockDataList.Where(blockData => blockData.Cost < 0))
            {
                blockData.Cost = 0;
            }
        }

        public void SetCurrentByName(string blockName)
        {
            var next =  blockDataList.FirstOrDefault(blockData => blockData.Name == blockName);
            if (next == null) return;
            if (next == Current) return;
            Current = next;
        }
    }
}