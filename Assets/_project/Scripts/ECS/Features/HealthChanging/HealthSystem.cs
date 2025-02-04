using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.HealthChanging
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(HealthSystem))]
    public sealed class HealthSystem : FixedUpdateSystem 
    {
        private Filter _healthDecreaseRequestFilter;
        
        private Stash<Health> _healthStash;
        private Stash<HealthChangeRequest> _healthChangeRequestStash;
    
        public override void OnAwake()
        {
            _healthStash = World.GetStash<Health>();
            
            _healthDecreaseRequestFilter = World.Filter.With<HealthChangeRequest>().Build();
            _healthChangeRequestStash = World.GetStash<HealthChangeRequest>();
        }

        public override void OnUpdate(float deltaTime)
        {
            // Находит все запросы на изменение здоровья, применяет их и удаляет
            foreach (var entity in _healthDecreaseRequestFilter)
            {
                ref var request = ref _healthChangeRequestStash.Get(entity);
                ref var health = ref _healthStash.Get(request.TargetEntity);
                health.HealthPoints += request.Amount;
            }
            
            _healthChangeRequestStash.RemoveAll();
        }
    }
}