using _project.Scripts.ECS.Features.HealthChanging;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.GameCoreEntity
{
    /// <summary>
    /// Если здоровье хотя бы одной GameCoreEntity опустилось до нуля, создаётся запрос на завершение игры 
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(GameCoreEntitySystem))]
    public sealed class GameCoreEntitySystem : FixedUpdateSystem
    {
        private Filter _gameCoreEntities;
        
        private Stash<GameCoreEntityDeathEvent> _deathEventStash;
        
        public override void OnAwake()
        {
            _gameCoreEntities = World.Filter
                .With<GameCoreEntity>()
                .With<Health>()
                .Build();

            _deathEventStash = World.GetStash<GameCoreEntityDeathEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            _deathEventStash.RemoveAll();
            
            var gameCoreEntityNoHealth = false;

            foreach (var entity in _gameCoreEntities)
            {
                ref var health = ref entity.GetComponent<Health>();
                
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