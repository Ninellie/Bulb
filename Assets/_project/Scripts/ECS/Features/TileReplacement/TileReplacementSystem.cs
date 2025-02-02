using System.Collections.Generic;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.BlocksToolbarPanel;
using _project.Scripts.ECS.Features.ChitinPayment;
using _project.Scripts.ECS.Features.MultipleTileSelection;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    public struct TilesChangeRequest : IComponent
    {
        public List<TileChangeData> TileChangeData;
    }
    
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(TileReplacementSystem))]
    public sealed class TileReplacementSystem : UpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private BlockDataPreset blockDataPreset;
        
        private HashSet<Vector3Int> _selectedPositions;
        
        private Stash<BlockButtonClickEvent> _toolbarItemClickEventStash;
        private Stash<SelectTilesEvent> _selectTilesEventStash;
        private Stash<TilesChangeRequest> _tilesChangeRequestStash;
        private Stash<PaymentRequest> _paymentRequestStash;
        
        private Filter _paidTileChangeRequestFilter;
        
        
        public override void OnAwake()
        {
            _selectedPositions = new HashSet<Vector3Int>();
            
            _toolbarItemClickEventStash = World.GetStash<BlockButtonClickEvent>();
            _selectTilesEventStash = World.GetStash<SelectTilesEvent>();
            _tilesChangeRequestStash = World.GetStash<TilesChangeRequest>();
            _paymentRequestStash = World.GetStash<PaymentRequest>();
            
            _paidTileChangeRequestFilter = World.Filter
                .With<TilesChangeRequest>()
                .With<PaymentSuccess>()
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            UpdateSelectedPositions();
            
            // Обработать оплаченные запросы и удалить все запросы
            HandleSuccessfullyPayedRequests();
            
            // Создать новые запросы
            CreateTileChangeRequests();
        }

        private void UpdateSelectedPositions()
        {
            foreach(ref var selectEvent in _selectTilesEventStash)
            {
                _selectedPositions = selectEvent.SelectionPositions;
            }
        }
        
        private void CreateTileChangeRequests()
        {
            foreach (ref var clickEvent in _toolbarItemClickEventStash)
            {
                var blockTileName = clickEvent.BlockTileName;
                var blockData = blockDataPreset.GetBlockDataByName(blockTileName);

                if (blockData == null) continue;
                
                var changeDataList = new List<TileChangeData>();
                
                foreach (var position in _selectedPositions)
                {
                    var selectedTile = tilemap.value.GetTile(position);
                    
                    // Проверить не одинаковый ли блок заменяется
                    if (selectedTile == blockData.TileBasePrefab) continue;
                    
                    var tileChangeData = new TileChangeData(position
                        ,blockData.TileBasePrefab
                        ,Color.black
                        ,Matrix4x4.identity);
                    
                    changeDataList.Add(tileChangeData);
                }
                
                var entity = World.CreateEntity();
                
                ref var tilesChangeRequest  = ref _tilesChangeRequestStash.Add(entity);
                tilesChangeRequest.TileChangeData = changeDataList;
                
                ref var paymentRequest  = ref _paymentRequestStash.Add(entity);
                paymentRequest.Cost = blockData.Cost * changeDataList.Count;
                
            }
        }

        private void HandleSuccessfullyPayedRequests()
        {
            foreach (var entity in _paidTileChangeRequestFilter)
            {
                ref var tilesChangeRequest = ref entity.GetComponent<TilesChangeRequest>();
                
                if (tilesChangeRequest.TileChangeData == null)
                {
                    continue;
                }
                
                foreach (var tileChangeData in tilesChangeRequest.TileChangeData)
                {
                    tilemap.value.SetTile(tileChangeData, true);
                }
            }
            
            _tilesChangeRequestStash.RemoveAll();
        }
    }
}