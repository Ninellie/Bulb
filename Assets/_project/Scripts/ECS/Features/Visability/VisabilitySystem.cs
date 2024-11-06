using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Visability
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update" + nameof(VisabilitySystem))]
    public sealed class VisabilitySystem : UpdateSystem
    {
        private Filter _renderedFilter;
        private Stash<Rendered> _rendered;
        private Stash<Rendered> _visible;
        private Stash<Rendered> _invisible;

        public override void OnAwake()
        {
            _renderedFilter = World.Filter.With<Rendered>().Build();
            _rendered = World.GetStash<Rendered>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _renderedFilter)
            {
                if (!_rendered.Has(entity))
                {
                    continue;
                }
                
                ref var rendered = ref entity.GetComponent<Rendered>();

                var isVisible = rendered.Renderer.isVisible;

                if (isVisible)
                {
                    if (_invisible.Has(entity))
                    {
                        entity.RemoveComponent<Invisible>();
                    }
                    if (!_visible.Has(entity))
                    {
                        entity.AddComponent<Visible>();
                    }
                }
                else
                {
                    if (_visible.Has(entity))
                    {
                        entity.RemoveComponent<Visible>();
                    }
                    if (!_invisible.Has(entity))
                    {
                        entity.AddComponent<Invisible>();
                    }
                }
            }
        }
    }
}