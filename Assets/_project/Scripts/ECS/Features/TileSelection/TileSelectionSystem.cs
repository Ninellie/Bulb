using Core.Variables;
using Scellecs.Morpeh.Systems;
using TriInspector;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _project.Scripts.ECS.Features.TileSelection
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(TileSelectionSystem))]
    public sealed class TileSelectionSystem : UpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private GameObject tileSelection;
        [SerializeField] private InputActionAsset inputActionAsset;

        [SerializeField] [ReadOnly] private Vector3Int currentSelection;

        [SerializeField] private float multiSwapThreshold = 1f;
        [SerializeField] private float pressingSelectionChangeCooldown = 0.2f;
        
        private Transform _tileSelection;
        
        private InputAction _changeSelectionToLeft;
        private InputAction _changeSelectionToRight;
        private InputAction _changeSelectionToUp;
        private InputAction _changeSelectionToDown;

        [SerializeField] [ReadOnly] private float _pressTimer;
        [SerializeField] [ReadOnly] private float _pressingSelectionChangeCooldownTimer;
        
        public override void OnAwake()
        {
            _tileSelection = Instantiate(tileSelection).transform;
            currentSelection = tilemap.value.origin;
            var cellToWorld = tilemap.value.CellToWorld(currentSelection);
            _tileSelection.transform.position = cellToWorld;

            _pressingSelectionChangeCooldownTimer = pressingSelectionChangeCooldown;
            _pressTimer = 0;
            
            _changeSelectionToLeft = inputActionAsset.FindAction("ChangeSelectionToLeft");
            _changeSelectionToRight = inputActionAsset.FindAction("ChangeSelectionToRight");
            _changeSelectionToUp = inputActionAsset.FindAction("ChangeSelectionToUp");
            _changeSelectionToDown = inputActionAsset.FindAction("ChangeSelectionToDown");
            //changeSelectionToDown = inputActionAsset.FindAction("DeleteTile");
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_pressingSelectionChangeCooldownTimer > 0)
            {
                _pressingSelectionChangeCooldownTimer -= deltaTime;
            }
            UpdatePressingTimer(deltaTime);
            
            // Получить направление отжатых в этот кадр клавиш
            var selectionChange = GetReleasedSelectionChange();
            
            // Если никаких клавиш в этом кадре не отжато
            if (selectionChange.Equals(Vector3Int.zero))
            {
                // Если таймер зажатия клавиш больше чем трешхолд
                if (!(_pressTimer > multiSwapThreshold)) return;
                // Если кулдаун на смену позиции во время зажатия меньше нуля
                if (_pressingSelectionChangeCooldownTimer > 0) return;

                selectionChange = GetPressedSelectionChange();
                ChangeSelection(selectionChange);
                _pressingSelectionChangeCooldownTimer = pressingSelectionChangeCooldown;
                return;
            }

            _pressTimer = 0;
            
            ChangeSelection(selectionChange);
        }
        
        
        private void ChangeSelection(Vector3Int selectionChange)
        {
            var newSelection = currentSelection + selectionChange;

            if (!tilemap.value.HasTile(newSelection))
            {
                return;
            }

            currentSelection = newSelection;
                
            var cellToWorld = tilemap.value.CellToWorld(currentSelection);
            _tileSelection.transform.position = cellToWorld;
        }

        private Vector3Int GetReleasedSelectionChange()
        {
            var selectionChange = Vector3Int.zero;
            if (_changeSelectionToLeft.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.left;
            }
            if (_changeSelectionToRight.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.right;
            }
            if (_changeSelectionToUp.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.up;
            }
            if (_changeSelectionToDown.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.down;
            }

            return selectionChange;
        }
        
        private Vector3Int GetPressedSelectionChange()
        {
            var selectionChange = Vector3Int.zero;
            if (_changeSelectionToLeft.IsPressed())
            {
                selectionChange += Vector3Int.left;
            }
            if (_changeSelectionToRight.IsPressed())
            {
                selectionChange += Vector3Int.right;
            }
            if (_changeSelectionToUp.IsPressed())
            {
                selectionChange += Vector3Int.up;
            }
            if (_changeSelectionToDown.IsPressed())
            {
                selectionChange += Vector3Int.down;
            }

            return selectionChange;
        }

        private void UpdatePressingTimer(float deltaTime)
        {
            if (_changeSelectionToLeft.IsPressed())
            {
                _pressTimer += deltaTime;
                return;
            }
            
            if (_changeSelectionToRight.IsPressed())
            {
                _pressTimer += deltaTime;
                return;
            }
            
            if (_changeSelectionToUp.IsPressed())
            {
                _pressTimer += deltaTime;
                return;
            }
            
            if (_changeSelectionToDown.IsPressed())
            {
                _pressTimer += deltaTime;
            }
        }
    }
}