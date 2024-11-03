using _project.Scripts.ECS.Features.Spawner;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Health
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(HealthSystem))]
    public sealed class HealthSystem : FixedUpdateSystem 
    {
        private Filter _healthFilter;
        private Filter _healthDecreaseRequestFilter;
        private Stash<HealthComponent> _healthStash;
        private Stash<HealthChangeRequest> _healthDecreaseRequestStash;
    
        public override void OnAwake()
        {
            _healthFilter = World.Filter.With<HealthComponent>().Build();
            _healthStash = World.GetStash<HealthComponent>();
            
            _healthDecreaseRequestFilter = World.Filter.With<HealthChangeRequest>().Build();
            _healthDecreaseRequestStash = World.GetStash<HealthChangeRequest>();
        }

        public override void OnUpdate(float deltaTime)
        {
            // Находит все запросы на изменение здоровья, применяет их и удаляет
            foreach (var entity in _healthDecreaseRequestFilter)
            {
                ref var request = ref _healthDecreaseRequestStash.Get(entity);
                ref var health = ref _healthStash.Get(request.TargetEntity);
                health.HealthPoints += request.Amount;
                entity.RemoveComponent<HealthChangeRequest>();
            }
            
            // Находит все сущности с нулевым здоровьем и создаёт запрос на возвращение в пул
            foreach (var entity in _healthFilter)
            {
                ref var healthComponent = ref _healthStash.Get(entity);
                if (healthComponent.HealthPoints > 0)
                {
                    continue;
                }
                entity.AddComponent<ReleaseRequest>();
            }
        }
    }
}