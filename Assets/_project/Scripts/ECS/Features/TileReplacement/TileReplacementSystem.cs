using System.Collections.Generic;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.BlocksToolbarPanel;
using _project.Scripts.ECS.Features.MultipleTileSelection;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(TileReplacementSystem))]
    public sealed class TileReplacementSystem : UpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private BlockDataPreset blockDataPreset;
        
        private HashSet<Vector3Int> _selectedTiles;
        private Stash<BlockButtonClickEvent> _toolbarItemClickEventStash;
        private Stash<SelectTilesEvent> _selectTilesEventStash;
        
        public override void OnAwake()
        {
            _toolbarItemClickEventStash = World.GetStash<BlockButtonClickEvent>();
            _selectTilesEventStash = World.GetStash<SelectTilesEvent>();
            _selectedTiles = new HashSet<Vector3Int>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach(ref var selectEvent in _selectTilesEventStash)
            {
                _selectedTiles = selectEvent.SelectionPositions;
            }
            
            foreach (ref var clickEvent in _toolbarItemClickEventStash)
            {
                var blockTileName = clickEvent.BlockTileName;
                var blockData = blockDataPreset.GetBlockDataByName(blockTileName);

                if (blockData == null) continue;

                foreach (var selectedTile in _selectedTiles)
                {
                    var selectedTileBase = tilemap.value.GetTile(selectedTile);
                    
                    // Проверить не одинаковый ли блок заменяется
                    if (selectedTileBase == blockData.TileBasePrefab) continue;
                    
                    // Заменить блок в тайлмапе
                    var tileChangeData = new TileChangeData(selectedTile
                        ,blockData.TileBasePrefab
                        ,Color.black
                        ,Matrix4x4.identity);
                    
                    tilemap.value.SetTile(tileChangeData, true);
                }
                
                // Проверить хватает ли ресурсов (хитина) 
                //if (currentChitin == null) return;
                //if (selectedBlockData.Cost > currentChitin.value) return;
                
                // Отнять нужное количество хитина
                //currentChitin.ApplyChange(selectedBlockData.Cost);
            }
        }
    }
}