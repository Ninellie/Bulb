using _project.Scripts.ECS.Features.ObjectDestroy;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MovementSystem))]
    public sealed class MovementSystem : FixedUpdateSystem
    {
        private Filter _filter;
        private Stash<Movable> _movableStash;

        public override void OnAwake()
        {
            _filter = World.Filter.With<Movable>().With<GameObjectComponent>().Build();
            _movableStash = World.GetStash<Movable>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var transform = _movableStash.Get(entity).Transform;
                var targetPosition = _movableStash.Get(entity).TargetPosition.Value;
                var speed = _movableStash.Get(entity).Speed.Value;

                var position = transform.position;
                var direction = (targetPosition - (Vector2)position).normalized;
                direction *= speed * deltaTime;
                
                var nextPos = (Vector2)position + direction;
                position = nextPos;
                transform.position = position;
            }
        }
    }
}