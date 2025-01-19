using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(BlocksToolBarPanelSystem))]
    public class BlocksToolBarPanelSystem : UpdateSystem
    {
        private Filter _blockButtonClickedFilter;
        private Stash<BlockButtonClickEvent> _blockButtonClickedEventStash;
        
        public override void OnAwake()
        {
            _blockButtonClickedFilter = World.Filter
                .With<BlockButton>()
                .With<ButtonClicked>()
                .Build();

            _blockButtonClickedEventStash = World.GetStash<BlockButtonClickEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
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