using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.ChitinIncome
{
    /// <summary>
    /// Хитин - игровая валюта используемая для постройки блоков
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(ChitinIncomeSystem))]
    public sealed class ChitinIncomeSystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable currentCurrency;
        
        private const float StartChitinIncome = 10f;
        
        private Stash<EnemyDeathEvent> _enemyDeathEventStash;
        
        public override void OnAwake()
        {
            _enemyDeathEventStash = World.GetStash<EnemyDeathEvent>();
            currentCurrency.SetValue(StartChitinIncome);
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var returnEvent in _enemyDeathEventStash)
            {
                currentCurrency.value += 1;
            }
        }
    }
}