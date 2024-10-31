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
        private Filter _filter;
        private Stash<Health> _healthStash;
    
        public override void OnAwake()
        {
            _filter = World.Filter.With<Health>().Build();
            _healthStash = World.GetStash<Health>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
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