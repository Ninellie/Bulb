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
        
        private Transform _tileSelection;
        
        public InputAction changeSelectionToLeft;
        public InputAction changeSelectionToRight;
        public InputAction changeSelectionToUp;
        public InputAction changeSelectionToDown;
        
        public override void OnAwake()
        {
            _tileSelection = Instantiate(tileSelection).transform;
            currentSelection = tilemap.value.origin;
            var cellToWorld = tilemap.value.CellToWorld(currentSelection);
            _tileSelection.transform.position = cellToWorld;
            
            changeSelectionToLeft = inputActionAsset.FindAction("ChangeSelectionToLeft");
            changeSelectionToRight = inputActionAsset.FindAction("ChangeSelectionToRight");
            changeSelectionToUp = inputActionAsset.FindAction("ChangeSelectionToUp");
            changeSelectionToDown = inputActionAsset.FindAction("ChangeSelectionToDown");
        }

        public override void OnUpdate(float deltaTime)
        {
            var selectionChange = Vector3Int.zero;

            if (changeSelectionToLeft.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.left;
            }
            if (changeSelectionToRight.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.right;
            }
            if (changeSelectionToUp.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.up;
            }
            if (changeSelectionToDown.WasReleasedThisFrame())
            {
                selectionChange += Vector3Int.down;
            }

            if (selectionChange.Equals(Vector3Int.zero))
            {
                return;
            }
            
            currentSelection += selectionChange;
            
            var cellToWorld = tilemap.value.CellToWorld(currentSelection);
            _tileSelection.transform.position = cellToWorld;
        }
    }
}