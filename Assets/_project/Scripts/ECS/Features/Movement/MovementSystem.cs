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
            _filter = World.Filter.With<Movable>().Build();
            _movableStash = World.GetStash<Movable>();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var movableTransform = _movableStash.Get(entity).Transform;
                var directionAsTarget = _movableStash.Get(entity).DirectionAsTarget;
                var direction = _movableStash.Get(entity).Direction.Value;
                var speed = _movableStash.Get(entity).Speed.Value;
                var speedScale = _movableStash.Get(entity).SpeedScale;
                
                var movablePosition = (Vector2)movableTransform.position;

                if (directionAsTarget)
                {
                    direction = (direction - movablePosition).normalized;
                }
                else
                {
                    direction.Normalize();
                }
                
                direction *= speed * deltaTime * speedScale;
                
                var nextPos = movablePosition + direction;
                movablePosition = nextPos;
                movableTransform.position = movablePosition;
            }
        }
    }
}