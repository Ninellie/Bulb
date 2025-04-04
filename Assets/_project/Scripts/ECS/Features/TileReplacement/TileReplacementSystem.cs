﻿using System.Collections.Generic;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.Blocks;
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
        private Stash<BlockChangeEvent> _blockChangeEventStash;
        
        private Filter _paidTileChangeRequestFilter;
        
        public override void OnAwake()
        {
            _selectedPositions = new HashSet<Vector3Int>();
            
            _toolbarItemClickEventStash = World.GetStash<BlockButtonClickEvent>();
            _selectTilesEventStash = World.GetStash<SelectTilesEvent>();
            _tilesChangeRequestStash = World.GetStash<TilesChangeRequest>();
            _paymentRequestStash = World.GetStash<PaymentRequest>();
            _blockChangeEventStash = World.GetStash<BlockChangeEvent>();
            
            _paidTileChangeRequestFilter = World.Filter
                .With<TilesChangeRequest>()
                .With<PaymentSuccess>()
                .Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            // Удалить все события о смене блока
            _blockChangeEventStash.RemoveAll();
            
            // Обновить позиции выделенных блоков
            UpdateSelectedPositions();
            
            // Обработать оплаченные запросы на изменение и удалить все запросы
            HandleSuccessfullyPayedRequests();
            _tilesChangeRequestStash.RemoveAll();
            
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

                var cost = blockData.Cost * changeDataList.Count;

                if (cost > 0)
                {
                    var entity = World.CreateEntity();
                    
                    ref var tilesChangeRequest  = ref _tilesChangeRequestStash.Add(entity);
                    tilesChangeRequest.TileChangeData = changeDataList;
                    
                    ref var paymentRequest  = ref _paymentRequestStash.Add(entity);
                    paymentRequest.Cost = blockData.Cost * changeDataList.Count;
                }
                else
                {
                    ChangeTiles(changeDataList);
                }
            }
        }

        private void HandleSuccessfullyPayedRequests()
        {
            foreach (var entity in _paidTileChangeRequestFilter)
            {
                ref var tilesChangeRequest = ref entity.GetComponent<TilesChangeRequest>();
                
                ChangeTiles(tilesChangeRequest.TileChangeData);
            }
        }

        private void ChangeTiles(List<TileChangeData> tileChangeDataList)
        {
            if (tileChangeDataList == null)
            {
                return;
            }
            
            foreach (var tileChangeData in tileChangeDataList)
            {
                tilemap.value.SetTile(tileChangeData, true);
                CreateChangeEvent(tileChangeData);
            }
        }

        private void CreateChangeEvent(TileChangeData tileChangeData)
        {
            var oldTile = tilemap.value.GetTile(tileChangeData.position);
            var eventEntity = World.CreateEntity();
            ref var changeEvent = ref eventEntity.AddComponent<BlockChangeEvent>();
            changeEvent.Position = tileChangeData.position;
            changeEvent.NewBlockData = blockDataPreset.GetBlockDataByTile(tileChangeData.tile);
            changeEvent.OldBlockData = blockDataPreset.GetBlockDataByTile(oldTile);

            var tileGameObject = tilemap.value.GetInstantiatedObject(tileChangeData.position);
            var blockNameProvider = tileGameObject.GetComponent<BlockNameProvider>(); 
            var entity = blockNameProvider.Entity;
            changeEvent.NewEntity = entity;
        }
    }
}