using System.Collections.Generic;
using System.Linq;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Stats.MovementSpeed
{
    /// <summary>
    /// Отвечает за изменение и пересчёт значения стата MovementSpeedStat
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/Stats/" + nameof(MovementSpeedStatCalculatingSystem))]
    public sealed class MovementSpeedStatCalculatingSystem : FixedUpdateSystem
    {
        private Stash<MovementSpeedStat> _stats;
        private Stash<SelfMovementSpeedStatChangeEvent> _statChangeEvents;
        private Stash<MovementSpeedStatAddModRequest> _addModRequests;
        private Stash<MovementSpeedStatRemoveModRequest> _removeModRequests;
        
        public override void OnAwake()
        {
            _stats = World.GetStash<MovementSpeedStat>();
            _statChangeEvents = World.GetStash<SelfMovementSpeedStatChangeEvent>();
            _addModRequests = World.GetStash<MovementSpeedStatAddModRequest>();
            _removeModRequests = World.GetStash<MovementSpeedStatRemoveModRequest>();
        }

        public override void OnUpdate(float deltaTime)
        {
            // Очистить стеш ивентов
            _statChangeEvents.RemoveAll();
            
            var entitiesToRecalculate = new List<Entity>();
            
            foreach (var request in _addModRequests)
            {
                var stat = _stats.Get(request.Target);
                stat.Mods.Add(request.StatMod);
                entitiesToRecalculate.Add(request.Target);
            }
            _addModRequests.RemoveAll();

            foreach (var request in _removeModRequests)
            {
                var stat = _stats.Get(request.Target);
                stat.Mods.Remove(request.StatMod);
                entitiesToRecalculate.Add(request.Target);
            }
            
            _removeModRequests.RemoveAll();

            foreach (var entity in entitiesToRecalculate)
            {
                Recalculate(entity);
            }
        }

        private void Recalculate(Entity entity)
        {
            ref var stat = ref _stats.Get(entity);
            var flatModsValue = stat.Mods.Where(statMod => statMod.Type == StatMod.StatModType.Flat).Sum(statMod => statMod.Value);
            var percentageModsValue = stat.Mods.Where(statMod => statMod.Type == StatMod.StatModType.Percentage).Sum(statMod => statMod.Value);
            var flat = stat.Base + flatModsValue;
            var percentage = (percentageModsValue / 100) + 1;
            var value = flat * percentage;
            stat.Current = Mathf.Clamp(value, stat.Min, stat.Max);
            var prevStatValue = stat.Current;
            if (prevStatValue == stat.Current) return;
            var eventEntity = World.CreateEntity();
            _statChangeEvents.Add(eventEntity, new SelfMovementSpeedStatChangeEvent { Value = stat.Current });
        }
    }
}