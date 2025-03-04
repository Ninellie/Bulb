using _project.Scripts.ECS.Features.TileReplacement;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(BlocksToolBarPanelSystem))]
    public sealed class BlocksToolBarPanelSystem : UpdateSystem
    {
        [SerializeField] private BlockDataPreset blockDataPreset;
        [SerializeField] private GameObject toolBarPanelPrefab;
        [SerializeField] private BlockButtonProvider buttonPrefab;
        
        private Filter _blockButtonClickedFilter;
        private Stash<BlockButtonClickEvent> _blockButtonClickedEventStash;
        
        public override void OnAwake()
        {
            _blockButtonClickedFilter = World.Filter
                .With<BlockButton>()
                .With<ButtonClicked>()
                .Build();

            _blockButtonClickedEventStash = World.GetStash<BlockButtonClickEvent>();
            
            var toolBarPanelLayout = Instantiate(toolBarPanelPrefab).GetComponentInChildren<VerticalLayoutGroup>();
            
            foreach (var blockData in blockDataPreset.GetBlockData())
            {
                var blockName = blockData.Name;
                var blockCost = blockData.Cost;
                var blockPicture = blockData.Picture;

                var button = Instantiate(buttonPrefab, toolBarPanelLayout.transform, false);
                button.name = blockName;
                ref var blockButton = ref button.Entity.GetComponent<BlockButton>();
                blockButton.BlockTileName = blockName;
                button.gameObject.GetComponent<Image>().sprite = blockPicture;
                button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = blockCost.ToString();
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            _blockButtonClickedEventStash.RemoveAll();
            
            // Если кнопка была нажата в этом кадре, создать компонент-ивент о нажатии определённой кнопки
            foreach (var entity in _blockButtonClickedFilter)
            {
                ref var blockButton = ref entity.GetComponent<BlockButton>();
                var blockTileName = blockButton.BlockTileName;
                
                var eventEntity = World.CreateEntity();
                var blockButtonClickedEvent = new BlockButtonClickEvent { BlockTileName = blockTileName };
                _blockButtonClickedEventStash.Set(eventEntity, blockButtonClickedEvent);

                entity.RemoveComponent<ButtonClicked>();
            }
        }
    }
}