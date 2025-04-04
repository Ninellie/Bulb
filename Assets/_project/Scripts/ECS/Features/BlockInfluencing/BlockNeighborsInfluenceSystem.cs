using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.BlockNeighboursSetting;
using _project.Scripts.ECS.Features.Blocks;
using _project.Scripts.ECS.Features.Stats;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlockInfluencing
{
    /// <summary>
    /// Реагирует на события обновления блоков на tilemap и обновляет их эффекты.
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/Effects" + nameof(BlockNeighborsInfluenceSystem))]
    public sealed class BlockNeighborsInfluenceSystem : FixedUpdateSystem
    {
        private Stash<UpdateNeighboursInfluenceRequest> _updateRequestStash;
        private Stash<Neighboring> _neighbouringStash;
        private Stash<BlockName> _blockNameStash;
        
        public override void OnAwake()
        {
            _updateRequestStash = World.GetStash<UpdateNeighboursInfluenceRequest>();
            _neighbouringStash = World.GetStash<Neighboring>();
            _blockNameStash = World.GetStash<BlockName>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var request in _updateRequestStash)
            {
                HandleUpdateRequest(request);
            }
        }

        private void HandleUpdateRequest(UpdateNeighboursInfluenceRequest request)
        {
            //var position = changeEvent.Position;
            var oldTileData = request.OldBlockData;
            var newTileData = request.NewBlockData;
            var entity = request.NewEntity;
            var effectsToRemove = oldTileData.Effects; 
            var effectsToApply = newTileData.Effects;
            
            ref var neighboring = ref _neighbouringStash.Get(entity);
            var neighbours = neighboring.Neighbors;
            
            // Для каждого эффекта к убиранию
            RemoveEffects(effectsToRemove, neighbours, entity);
            ApplyEffects(effectsToApply, neighbours, entity);
        }

        private void RemoveEffects(List<BlockEffect> effectsToRemove, List<Neighbor> neighbours, Entity entity)
        {
            ChangeEffects(effectsToRemove, neighbours, entity, false);
        }
        
        private void ApplyEffects(List<BlockEffect> effectsToAdd, List<Neighbor> neighbours, Entity entity)
        {
            ChangeEffects(effectsToAdd, neighbours, entity, true);
        }

        private void ChangeEffects(List<BlockEffect> effects, List<Neighbor> neighbours, Entity entity, bool isApply)
        {
            foreach (var effect in effects)
            {
                // Для каждого соседа
                foreach (var neighbour in neighbours)
                {
                    // Проверка действует ли на соседа данный эффект
                    var blockName = _blockNameStash.Get(entity).Name;
                    if (effect.AffectingBlockID.All(s => s != blockName)) continue;
                    // Создать запрос добавления эффекта с соседа
                    foreach (var statEffect in effect.StatMods)
                    {
                        if (isApply)
                        {
                            StatUtils.CreateStatModAddRequest(
                                World, 
                                neighbour.Entity, 
                                statEffect.Mod, 
                                statEffect.StatId);
                        }
                        else
                        {
                            StatUtils.CreateStatModRemoveRequest(
                                World, 
                                neighbour.Entity, 
                                statEffect.Mod, 
                                statEffect.StatId);
                        }
                    }
                }
            }
        }
    }
}