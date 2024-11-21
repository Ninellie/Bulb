using _project.Scripts.Core.Variables;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(TileReplacementSystem))]
    public sealed class TileReplacementSystem : UpdateSystem
    {
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private TilemapVariable tilemap;
        [SerializeField] private Vector3IntVariable currentSelection;
        [SerializeField] private BlockDataPreset blockDataPreset;
        [SerializeField] private IntVariable currentChitin;
        
        private InputAction _replaceTile;
    
        public override void OnAwake()
        {
            if (inputActionAsset == null) return;
            _replaceTile = inputActionAsset.FindAction("ReplaceBlock");
        }

        public override void OnUpdate(float deltaTime)
        {
            // При нужном инпуте
            if (inputActionAsset == null) return;
            if (_replaceTile == null) return;
            var isReleasedThisFrame = _replaceTile.WasReleasedThisFrame();
            if (!isReleasedThisFrame) return;
        
            // Получить тайлмап
            if (tilemap == null) return;
            if (tilemap.value == null) return;

            // Получить селекшен Vector2Int
            if (currentSelection == null) return;

            // Получить блок в тайлмапе в селекшене Vector2Int
            var selectedTileBase = tilemap.value.GetTile(currentSelection);

            // Получить выбранные данные для блока
            if (blockDataPreset == null) return;
            if (blockDataPreset.Current == null) return;
            var selectedBlockData = blockDataPreset.Current;

            // Проверить не одинаковый ли блок заменяется
            if (selectedTileBase == selectedBlockData.Prefab) return;

            // Проверить хватает ли ресурсов (хитина) 
            if (currentChitin == null) return;
            if (selectedBlockData.Cost > currentChitin.value) return;

            // Отнять нужное количество хитина
            currentChitin.ApplyChange(selectedBlockData.Cost);

            // Заменить блок в тайлмапе
            var tileChangeData = new TileChangeData(currentSelection, selectedTileBase, Color.black, Matrix4x4.identity);
            tilemap.value.SetTile(tileChangeData, true);
        }
    }
}