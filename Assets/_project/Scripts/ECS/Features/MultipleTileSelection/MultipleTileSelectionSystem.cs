using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Variables;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.MultipleTileSelection
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(MultipleTileSelectionSystem))]
    public sealed class MultipleTileSelectionSystem : UpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private LineRenderer lineRendererPrefab;
        [SerializeField] private List<Vector3Int> selectedTiles = new();
        
        private readonly HashSet<Vector3Int> _selectedTiles = new();
        private LineRenderer _lineRenderer;
        private bool _isSelecting;
        private Camera _mainCamera;
        
        private Vector3Int _startTilePos;
        private Vector3Int _endTilePos;
        
        public override void OnAwake()
        {
            _mainCamera = Camera.main;
            _lineRenderer = Instantiate(lineRendererPrefab);
        }
        
        public override void OnUpdate(float deltaTime)
        {
            selectedTiles = _selectedTiles.ToList();
            SetSelectingState();

            if (_isSelecting)
            {
                var tileUnderMouse = GetMouseTilePosition();
                _selectedTiles.Add(tileUnderMouse);
                GenerateOutline();
            }
            else
            {
                _selectedTiles.Clear();
            }
        }
        
        private void SetSelectingState()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isSelecting = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isSelecting = false;
            }
        }
        
        private Vector3Int GetMouseTilePosition()
        {
            // Получение позиции тайла под мышью
            var worldPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return tilemap.value.WorldToCell(worldPoint);
        }
        
        private void GenerateOutline()
        {
            _lineRenderer.positionCount = 0;
            // Создаем контур
            var tilesEdgeRect = GetTilesEdgeRect();

            var edgeRect = GetEdgeRect(tilesEdgeRect);
            
            var posList = new List<Vector3>();
            
            var leftDownCorner = edgeRect.min;
            var leftTopCorner = new Vector2(edgeRect.min.x, edgeRect.max.y);
            var rightDownCorner = new Vector2(edgeRect.max.x, edgeRect.min.y);
            var rightTopCorner = edgeRect.max;

            posList.Add(leftDownCorner);
            posList.Add(leftTopCorner);
            posList.Add(rightTopCorner);
            posList.Add(rightDownCorner);
            
            _lineRenderer.positionCount = 4;
            _lineRenderer.SetPositions(posList.ToArray());
        }

        private Rect GetEdgeRect(RectInt tilesRect)
        {
            var leftDownTile = (Vector3Int)tilesRect.min;
            var rightTopTile = (Vector3Int)tilesRect.max;
            
            var leftDownCorner = tilemap.value.CellToWorld(leftDownTile) + new Vector3(-4, -4);
            var rightTopCorner = tilemap.value.CellToWorld(rightTopTile) + new Vector3(4, 4);
            
            var size = rightTopCorner - leftDownCorner;
            return new Rect(leftDownCorner, size);
        }
        
        private RectInt GetTilesEdgeRect()
        {
            var leftSide = int.MaxValue;
            var downSide = int.MaxValue;
            var rightSide = int.MinValue;
            var upSide = int.MinValue;
            
            foreach (var tile in _selectedTiles)
            {
                if (tile.x < leftSide)
                {
                    leftSide = tile.x;
                }
                if (tile.x > rightSide)
                {
                    rightSide = tile.x;
                }
                if (tile.y < downSide)
                {
                    downSide = tile.y;
                }
                if (tile.y > upSide)
                {
                    upSide = tile.y;
                }
            }
            
            var position = new Vector2Int(leftSide, downSide);
            var size = new Vector2Int(Mathf.Abs(rightSide - leftSide), Mathf.Abs(upSide - downSide));
            var rect = new RectInt(position, size);

            return rect;
        }
    }
}