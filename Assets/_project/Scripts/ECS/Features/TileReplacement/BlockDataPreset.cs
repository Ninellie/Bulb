using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    [CreateAssetMenu(menuName = "ECS/Blocks/" + nameof(BlockDataPreset))]
    public class BlockDataPreset : ScriptableObject
    {
        [SerializeField] private List<BlockData> blockDataList;
        
        private void OnValidate()
        {
            foreach (var blockData in blockDataList.Where(blockData => blockData.Cost < 0))
            {
                blockData.Cost = 0;
            }
        }

        public BlockData GetBlockDataByName(string blockName)
        {
            return blockDataList.FirstOrDefault(blockData => blockData.Name == blockName);
        }

        public List<BlockData> GetBlockData()
        {
            return blockDataList;
        }
    }
}