using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.HealthChanging;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using UnityEngine.Serialization;

namespace _project.Scripts.ECS.Features.GameCoreEntity
{
    /// <summary>
    /// Если здоровье хотя бы одной GameCoreEntity опустилось до нуля, создаётся запрос на завершение игры 
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(GameCoreEntitySystem))]
    public sealed class GameCoreEntitySystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable healthCurrent;
        
        private const int StartCoreEntityHealth = 10;
        
        private Filter _gameCoreEntities;
        
        private Stash<GameCoreEntityDeathEvent> _deathEventStash;
        
        public override void OnAwake()
        {
            _gameCoreEntities = World.Filter
                .With<GameCoreEntity>()
                .With<Health>()
                .Build();

            _deathEventStash = World.GetStash<GameCoreEntityDeathEvent>();
            
            foreach (var entity in _gameCoreEntities)
            {
                ref var health = ref entity.GetComponent<Health>();
                health.HealthPoints = StartCoreEntityHealth;
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            _deathEventStash.RemoveAll();
            
            var gameCoreEntityNoHealth = false;

            foreach (var entity in _gameCoreEntities)
            {
                ref var health = ref entity.GetComponent<Health>();
                healthCurrent.value = health.HealthPoints;
                if (health.HealthPoints > 0)
                {
                    continue;
                }
                
                gameCoreEntityNoHealth = true;
            }

            if (gameCoreEntityNoHealth)
            {
                var eventEntity = World.CreateEntity();
                _deathEventStash.Add(eventEntity);
            }
        }
    }
}