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
        
        private Stash<SelectBoundsEvent> _selectEventsStash;
        
        private HashSet<Vector3Int> _selectedPositions = new();
        
        private Tilemap _outlineTilemap;
        
        public override void OnAwake()
        {
            _selectEventsStash = World.GetStash<SelectBoundsEvent>();
            _outlineTilemap = Instantiate(tilemapPrefab).GetComponentInChildren<Tilemap>();
        }
        
        public override void OnUpdate(float deltaTime)
        {
            //SetAddingState();
            foreach (ref var selectEvent in _selectEventsStash)
            {
                ClearSelection();
                var bounds = selectEvent.SelectionBounds;
                _positions = TileHelper.GetTilesWithinBounds(tilemap, bounds);
                GenerateOutline();
            }
        }

        private void ClearSelection()
        {
            foreach (var position in _selectedPositions)
            {
                _outlineTilemap.SetTile(position, null);
            }
        }
        
        private void GenerateOutline()
        {
            foreach (var position in _positions)
            {
                var pos = new Vector3Int(position.x, position.y, 10);
                _outlineTilemap.SetTile(pos, outlineTile);
                _selectedPositions.Add(pos);
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