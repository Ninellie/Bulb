using System.Collections.Generic;
using System.Linq;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Stats.EnergyGenRate
{
    /// <summary>
    /// Отвечает за изменение и пересчёт значения стата EnergyGenRate
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/Stats/" + nameof(EnergyGenRateStatCalculatingSystem))]
    public sealed class EnergyGenRateStatCalculatingSystem : FixedUpdateSystem
    {
        private Stash<EnergyGenRateStat> _stats;
        private Stash<SelfEnergyGenRateStatChangeEvent> _statChangeEvents;
        private Stash<EnergyGenRateStatAddModRequest> _addModRequests;
        private Stash<EnergyGenRateStatRemoveModRequest> _removeModRequests;
        
        public override void OnAwake()
        {
            _stats = World.GetStash<EnergyGenRateStat>();
            _statChangeEvents = World.GetStash<SelfEnergyGenRateStatChangeEvent>();
            _addModRequests = World.GetStash<EnergyGenRateStatAddModRequest>();
            _removeModRequests = World.GetStash<EnergyGenRateStatRemoveModRequest>();
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
            var prevStatValue = stat.Current;
            stat.Current = Mathf.Clamp(value, stat.Min, stat.Max);
            if (prevStatValue == stat.Current) return;
            var eventEntity = World.CreateEntity();
            _statChangeEvents.Add(eventEntity, new SelfEnergyGenRateStatChangeEvent { Value = stat.Current });
        }
    }
}