using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.CooldownReduction
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(CooldownReductionSystem))]
    public sealed class CooldownReductionSystem : FixedUpdateSystem
    {
        private Filter _nonFrozenCooldownFilter;
        private Stash<Cooldown> _cooldownStash;
        
        public override void OnAwake()
        {
            _nonFrozenCooldownFilter = World.Filter
                .With<Cooldown>()
                //.Without<Frozen>()
                .Build();
            _cooldownStash = World.GetStash<Cooldown>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _nonFrozenCooldownFilter)
            {
                ref var cooldown = ref _cooldownStash.Get(entity);
                cooldown.Current -= deltaTime;
                if (cooldown.Current > 0) continue;
                _cooldownStash.Remove(entity);
            }
        }
    }
}