using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.RectSelection
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(RectSelectionSystem))]
    public sealed class RectSelectionSystem : UpdateSystem
    {
        [SerializeField] private LineRenderer lineRendererPrefab;
        
        private Camera _mainCamera;
        private LineRenderer _lineRenderer;
        
        private Vector2 _startPosition;
        private Vector2 _currentPosition;
        private bool _isSelecting;
        
        private Stash<SelectBoundsEvent> _selectEventStash;
        
        public override void OnAwake()
        {
            _mainCamera = Camera.main;
            _lineRenderer = Instantiate(lineRendererPrefab);
            _lineRenderer.positionCount = 0;
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
                    ref var selectBoundsEvent = ref _selectEventStash.Add(entity);
                    selectBoundsEvent.SelectionBounds = _lineRenderer.bounds;
                    
                    _lineRenderer.positionCount = 0;
                    return;
                }
                
                _currentPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                
                GenerateOutline();
                
                return;
            }
            
            UpdateSelectingState();
            if (!_isSelecting) return;
            _startPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
        
        private void GenerateOutline()
        {
            _lineRenderer.positionCount = 0;
            var rectCorners = new[]
            {
                (Vector3)_startPosition,
                new Vector3(_startPosition.x, _currentPosition.y),
                (Vector3)_currentPosition,
                new Vector3(_currentPosition.x, _startPosition.y)
            };
            
            _lineRenderer.positionCount = 4;
            _lineRenderer.SetPositions(rectCorners);
        }
    }
}