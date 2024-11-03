using System;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;

namespace _project.Scripts.ECS.Features.Movement
{
    public sealed class MovableProvider : MonoProvider<Movable>
    {
        private void Awake()
        {
            ref var movable = ref Entity.GetComponent<Movable>();
            movable.Transform = transform;
        }
    }
}