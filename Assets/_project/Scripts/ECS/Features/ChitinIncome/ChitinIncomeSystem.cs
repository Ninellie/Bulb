using _project.Scripts.Core.Variables;
using _project.Scripts.ECS.Features.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.ChitinIncome
{
    [CreateAssetMenu(menuName = "ECS/Systems/Fixed/" + nameof(ChitinIncomeSystem))]
    public sealed class ChitinIncomeSystem : FixedUpdateSystem
    {
        [SerializeField] private FloatVariable currentCurrency;
        
        private Stash<EnemyReturnToPoolEvent> _returnToPoolEventStash;
        
        public override void OnAwake()
        {
            _returnToPoolEventStash = World.GetStash<EnemyReturnToPoolEvent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var returnEvent in _returnToPoolEventStash)
            {
                currentCurrency.value += 1;
            }
        }
    }
}