using System.Collections.Generic;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.RectSelection;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.MultipleTileSelection
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(MultipleTileSelectionSystem))]
    public sealed class MultipleTileSelectionSystem : UpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private GameObject tilemapPrefab;
        [SerializeField] private TileBase outlineTile;
        
        private HashSet<Vector3Int> _positions = new();
        //private bool _isAdding;
        
        private Tilemap _outlineTilemap;
        
        private Stash<SelectBoundsEvent> _selectEventsStash;
        private Stash<SelectTilesEvent> _selectTilesEventsStash;
        
        public override void OnAwake()
        {
            _selectEventsStash = World.GetStash<SelectBoundsEvent>();
            _selectTilesEventsStash = World.GetStash<SelectTilesEvent>();
            
            _outlineTilemap = Instantiate(tilemapPrefab).GetComponentInChildren<Tilemap>();
        }
        
        public override void OnUpdate(float deltaTime)
        {
            _selectTilesEventsStash.RemoveAll();
            
            //SetAddingState();
            foreach (ref var selectEvent in _selectEventsStash)
            {
                var bounds = selectEvent.SelectionBounds;
                var positions= TileHelper.GetTilesWithinBounds(tilemap, bounds); // Присваивание

                if (positions.Count == 0)
                {
                    continue;
                }
                
                ClearSelection();
                _positions = positions; 
                
                CreateTileSelectionEvent();
                
                GenerateOutline();
            }
        }

        private void CreateTileSelectionEvent()
        {
            var eventEntity = World.CreateEntity();
            var tileSelectionUpdateEvent = new SelectTilesEvent {SelectionPositions = _positions};
            _selectTilesEventsStash.Set(eventEntity, tileSelectionUpdateEvent);
        }
        
        private void ClearSelection()
        {
            foreach (var position in _positions)
            {
                _outlineTilemap.SetTile(position, null);
            }
        }
        
        private void GenerateOutline()
        {
            foreach (var position in _positions)
            {
                var pos = new Vector3Int(position.x, position.y);
                _outlineTilemap.SetTile(pos, outlineTile);
            }
        }
        //
        // private void SetAddingState()
        // {
        //     if (Input.GetKeyDown(KeyCode.LeftShift))
        //     {
        //         _isAdding = true;
        //     }
        //
        //     if (Input.GetKeyUp(KeyCode.LeftShift))
        //     {
        //         _isAdding = false;
        //     }
        // }
    }
}