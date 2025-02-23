using System.Collections.Generic;
using System.Text;
using _project.Scripts.ECS.Features.Blocks;
using _project.Scripts.ECS.Features.EntitySelect;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlockInfoDisplay
{
    /// <summary>
    /// Слушает события о выделении блоков и отображает их 
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(BlockInfoDisplaySystem))]
    public class BlockInfoDisplaySystem : UpdateSystem
    {
        private Filter _selectedBlockFilter;
        private Filter _deselectedBlockFilter;

        private Stash<BlockInfoDisplay> _blockInfoStash;

        public override void OnAwake()
        {
            _selectedBlockFilter = World.Filter
                .With<BlockName>()
                .With<OnSelfSelectEvent>()
                .Build();

            _deselectedBlockFilter = World.Filter
                .With<BlockName>()
                .With<OnSelfDeselectEvent>()
                .Build();

            _blockInfoStash = World.GetStash<BlockInfoDisplay>();
        }

        public override void OnUpdate(float deltaTime)
        {
            // Если в этом кадре не пришло новых событий, то ничего не делать.
            var haveUpdates = false;
            var haveSelectEvents = false;
            var haveDeselectEvents = false;

            var stats = new Dictionary<string, string>();

            foreach (var entity in _deselectedBlockFilter)
            {
                haveUpdates = true;
                haveDeselectEvents = true;
            }

            foreach (var entity in _selectedBlockFilter)
            {
                haveUpdates = true;
                haveSelectEvents = true;
                stats = BlockInfoUtils.GetBlockStats(entity);
            }

            if (haveUpdates == false)
            {
                return;
            }

            // Если не пришло новых событий селекта и пришло деселект, убрать текст
            if (haveDeselectEvents && !haveSelectEvents)
            {
                foreach (ref var blockInfo in _blockInfoStash)
                {
                    blockInfo.Text.SetText(string.Empty);
                }

                return;
            }

            if (haveSelectEvents)
            {
                foreach (ref var blockInfo in _blockInfoStash)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (var stat in stats)
                    {
                        stringBuilder.Append($"{stat.Key}: {stat.Value}\n");
                    }
                    
                    blockInfo.Text.SetText(stringBuilder);
                }
            }

        }

    }
}