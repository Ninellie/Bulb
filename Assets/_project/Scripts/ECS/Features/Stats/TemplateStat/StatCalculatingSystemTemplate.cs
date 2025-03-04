using System;
using System.Collections.Generic;
using System.Linq;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Stats.TemplateStat
{
    public class MovementSpeedStatProvider : MonoProvider<TemplateStat>
    {
    }
    
    /// <summary>
    /// Создаётся при изменении значения стата Template или его компонентов
    /// </summary>
    public struct SelfTemplateStatChangeEvent : IComponent
    {
        public float Value;
    }
    
    /// <summary>
    /// Создаётся для добавления модификатора
    /// </summary>
    public struct TemplateStatAddModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }
    
    /// <summary>
    /// Создаётся для удаления модификатора
    /// </summary>
    public struct TemplateStatRemoveModRequest : IComponent
    {
        public Entity Target;
        public StatMod StatMod;
    }

    /// <summary>
    /// Изменяется только внутри системы TemplateStatCalculatingSystem.
    /// Для изменения извне, другие системы могут вызывать TemplateStatAddModRequest и TemplateStatRemoveModRequest
    /// </summary>
    [Serializable]
    public struct TemplateStat : IComponent
    {
        public float Base;
        public float Current;
        public float Min;
        public float Max;
        public List<StatMod> Mods;
    }
    
    /// <summary>
    /// Отвечает за изменение и пересчёт значения стата TemplateStat
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(TemplateStatCalculatingSystem))]
    public sealed class TemplateStatCalculatingSystem : FixedUpdateSystem
    {
        private Stash<TemplateStat> _stats;
        private Stash<SelfTemplateStatChangeEvent> _statChangeEvents;
        private Stash<TemplateStatAddModRequest> _addModRequests;
        private Stash<TemplateStatRemoveModRequest> _removeModRequests;
        
        public override void OnAwake()
        {
            _stats = World.GetStash<TemplateStat>();
            _statChangeEvents = World.GetStash<SelfTemplateStatChangeEvent>();
            _addModRequests = World.GetStash<TemplateStatAddModRequest>();
            _removeModRequests = World.GetStash<TemplateStatRemoveModRequest>();
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
            _statChangeEvents.Add(eventEntity, new SelfTemplateStatChangeEvent { Value = stat.Current });
        }
    }
}