using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.RectSelection
{
    [CreateAssetMenu(menuName = "ECS/Systems/LateUpdate/" + nameof(GlRectSelectionSystem))]
    public sealed class GlRectSelectionSystem : LateUpdateSystem
    {
        private static readonly int SortingLayerID = Shader.PropertyToID("_SortingLayerID");
        
        [SerializeField] private Shader shader;
        
        private Camera _mainCamera;
        private Material _lineMaterial;
        private Mesh _lineMesh;
        
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private bool _isSelecting;
        private Bounds _selectionBounds;
        private Stash<SelectBoundsEvent> _selectEventStash;
        
        public override void OnAwake()
        {
            _mainCamera = Camera.main;
            
            _lineMaterial = new Material(shader)
            {
                color = Color.red,
                renderQueue = 5000
            };
            
            _lineMaterial.SetInt(SortingLayerID, SortingLayer.NameToID("OverUI"));
            _lineMesh = new Mesh();
            _selectEventStash = World.GetStash<SelectBoundsEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _selectEventStash.RemoveAll();
            
            if (_isSelecting)
            {
                UpdateSelectingState();

                // Окончание выделения
                if (!_isSelecting)
                {
                    // send SelectRectEvent
                    var entity = World.CreateEntity();
                    _selectionBounds = _lineMesh.bounds;
                     ref var selectBoundsEvent = ref _selectEventStash.Add(entity);
                    selectBoundsEvent.SelectionBounds = _selectionBounds; 
                    return;
                }

                _currentPosition = GetMouseWorldPosition();
                
                DrawRectangle();
                
                return;
            }
            
            UpdateSelectingState();
            
            if (!_isSelecting) return;
            _startPosition = GetMouseWorldPosition(); 
            _currentPosition = _startPosition;
        }

        private void UpdateSelectingState()
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

        private Vector2 GetMouseWorldPosition()
        {
            return _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        
        private void DrawRectangle()
        {
            var topLeft = new Vector2(_startPosition.x, _currentPosition.y);
            var topRight = new Vector2(_currentPosition.x, _currentPosition.y);
            var bottomRight = new Vector2(_currentPosition.x, _startPosition.y);
            var bottomLeft = new Vector2(_startPosition.x, _startPosition.y);

            Vector3[] vertices = { bottomLeft, bottomRight, topRight, topLeft };
            int[] indices = { 0, 1, 1, 2, 2, 3, 3, 0 }; // Соединение вершин линиями

            _lineMesh.Clear();
            _lineMesh.vertices = vertices;
            _lineMesh.SetIndices(indices, MeshTopology.Lines, 0);
            Graphics.DrawMesh(_lineMesh, Matrix4x4.identity, _lineMaterial, 0);
        }
        
        private void DrawLine()
        {
            Vector3[] vertices = { _startPosition, _currentPosition };
            int[] indices = { 0, 1 };

            _lineMesh.Clear();
            _lineMesh.vertices = vertices;
            _lineMesh.SetIndices(indices, MeshTopology.Lines, 0);
            Graphics.DrawMesh(_lineMesh, Matrix4x4.identity, _lineMaterial, 0);
        }
    }
}