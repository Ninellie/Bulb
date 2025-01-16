using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MovementSystem))]
    public sealed class MovementSystem : FixedUpdateSystem
    {
        private Filter _movableFilter;
        private Filter _movableTargetedFilter;
        private Stash<Movable> _movableStash;

        public override void OnAwake()
        {
            _movableFilter = World.Filter.With<Movable>().Build();
            
            _movableTargetedFilter = World.Filter
                .With<Movable>()
                .With<Targeted>()
                .Build();
            
            _movableStash = World.GetStash<Movable>();
        }

        public override void OnUpdate(float deltaTime)
        {
            UpdateDirections();

            foreach (var entity in _movableFilter)
            {
                MoveEntity(deltaTime, entity);
                ChangeSpeedScale(deltaTime, entity);
            }
        }

        private void UpdateDirections()
        {
            foreach (var entity in _movableTargetedFilter)
            {
                ref var movable = ref entity.GetComponent<Movable>();
                ref var targeted = ref entity.GetComponent<Targeted>();

                var targetPosition = targeted.TargetPosition.Value;
                
                var movableTransform = movable.Transform;
                var movablePosition = (Vector2)movableTransform.position;

                var direction = (targetPosition - movablePosition).normalized;
                movable.Direction = direction;
            }
        }

        private void MoveEntity(float deltaTime, Entity entity)
        {
            ref var movable = ref _movableStash.Get(entity);
            
            var movableTransform = movable.Transform;
            var direction = movable.Direction;
            var speed = movable.Speed.Value;
            var speedScale = movable.SpeedScale;

            var movablePosition = (Vector2)movableTransform.position;
            
            direction.Normalize();
            
            direction *= speed * deltaTime * speedScale;

            var nextPos = movablePosition + direction;
            movablePosition = nextPos;
            movableTransform.position = movablePosition;
        }
        
        private void ChangeSpeedScale(float deltaTime, Entity entity)
        {
            ref var movable = ref _movableStash.Get(entity);
            var speedScaleDecreasePerSecond = movable.SpeedScaleChangePerSecond;
            if (speedScaleDecreasePerSecond == 0) return;
            movable.SpeedScale += movable.SpeedScaleChangePerSecond * deltaTime;
        }
    }
}