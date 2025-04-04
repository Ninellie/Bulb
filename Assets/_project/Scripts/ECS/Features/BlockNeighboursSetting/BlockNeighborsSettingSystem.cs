using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.TileReplacement;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlockNeighboursSetting
{
    /// <summary>
    /// Реагирует на события о смене блока и устанавливает соседей для него,
    /// а также обновляет для его соседей этот блок.
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/Blocks" + nameof(BlockNeighborsSettingSystem))]
    public sealed class BlockNeighborsSettingSystem : FixedUpdateSystem
    {
        [SerializeField] private TilemapVariable tilemap;
        
        private Stash<BlockChangeEvent> _changeEvents;
        private Stash<Neighboring> _neighboring;
        
        public override void OnAwake()
        {
            _changeEvents = World.GetStash<BlockChangeEvent>();
            _neighboring = World.GetStash<Neighboring>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var changeEvent in _changeEvents)
            {
                // Получили событие о смене блока (уже поменялся)
                var position = changeEvent.Position;
                var newBlockEntity = changeEvent.NewEntity;

                // Получение соседей блока который поменялся
                var neighbors = GetNeighbors(position);
                
                ref var neighbouring = ref _neighboring.Get(newBlockEntity);
                neighbouring.Neighbors = neighbors;
                
                // Для каждого соседа
                foreach (var neighbor in neighbors)
                {
                    // Получить соседа
                    ref var neighbouringNeighbour = ref _neighboring.Get(neighbor.Entity);

                    if (neighbouringNeighbour.Neighbors == null)
                    {
                        Debug.LogError("No neighbouring neighbours found!");
                        return;
                    }
                    
                    var neighboursNeighbours = neighbouringNeighbour.Neighbors;
                    
                    
                    // Узнать его относительную позицию к блоку который поменялся
                    var changedBlockRelativePositionToNeighbour = neighbor.RelativePosition * -1;
                    
                    foreach (var neighboursNeighbour in neighboursNeighbours.Where(neighboursNeighbour => 
                                     neighboursNeighbour.RelativePosition == changedBlockRelativePositionToNeighbour))
                    {
                        // Найти нужного соседа.
                        // Заменить сущность соседа по данному направлению у этого соседа
                        neighboursNeighbour.Entity = newBlockEntity;
                    }
                }
            }
        }

        private List<Neighbor> GetNeighbors(Vector3Int position)
        {
            var neighbors = new List<Neighbor>();
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.Top));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.RightTop));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.Right));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.RightBottom));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.Bottom));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.LeftBottom));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.Left));
            neighbors.Add(NeighboursUtils.GetNeighbour(tilemap, position, NeighboursUtils.Direction.LeftTop));
            return neighbors;
        } 
    }
}