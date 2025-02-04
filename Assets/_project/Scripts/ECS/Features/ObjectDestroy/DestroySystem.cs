using _project.Scripts.ECS.Features.HealthChanging;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.ObjectDestroy
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DestroySystem))]
    public sealed class DestroySystem : FixedUpdateSystem
    {
        private Filter _filter;
        private Stash<Health> _healthStash;
    
        public override void OnAwake()
        {
            _filter = World.Filter.With<DestroyRequest>().With<GameObjectComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var gameObjectComponent = ref entity.GetComponent<GameObjectComponent>();
                Destroy(gameObjectComponent.GameObject);
            }
        }
    }
}