using _project.Scripts.ECS.Features.GameCoreEntity;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _project.Scripts.ECS.Features.GameReset
{
    /// <summary>
    ///  Отслеживает события смерти ключевых сущностей и перезагружает сцену, если находит.
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(GameResetSystem))]
    public sealed class GameResetSystem : FixedUpdateSystem
    {
        private Stash<GameCoreEntityDeathEvent> _deathEventStash;
        
        public override void OnAwake()
        {
            _deathEventStash = World.GetStash<GameCoreEntityDeathEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (_deathEventStash.Length <= 0) return;
            SceneManager.LoadScene(0);
        }
    }
}