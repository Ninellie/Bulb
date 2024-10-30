using System;
using System.Collections.Generic;
using _project.Scripts.ECS.Features.Spawner;
using Core.Shapes;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;

namespace _project.Scripts.ECS.Features.EnemyPlacing
{
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RandomPlacerSystem))]
    public sealed class RandomPlacerSystem : FixedUpdateSystem
    {
        [SerializeField] private CheckMode placeAreaCheckMode;
        
        private readonly List<IShape> _placeBlockAreas = new();
        private readonly List<IShape> _placeAreas = new();

        private Filter _filter;
        private Stash<RandomPlaceRequest> _requestStash;

        public override void OnAwake()
        {
            _filter = World.Filter.With<RandomPlaceRequest>().Build();
            _requestStash = World.GetStash<RandomPlaceRequest>();
            InitSpawnAreas();
        }

        public override void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                var point = RandomPointGenerator.GetRandomPoint(_placeAreas, 
                    _placeBlockAreas, placeAreaCheckMode);
                
                if (point == null)
                {
                    throw new NullReferenceException("Null enemy position from RandomPoint Generator");
                }
                _requestStash.Get(entity).ObjectTransform.position = (Vector3)point;
                entity.Dispose();
            }
        }
        
        private void InitSpawnAreas()
        {
            var cam = Camera.main;

            if (cam == null) throw new NullReferenceException("Main camera is null");
            
            var height = cam.scaledPixelHeight;
            var width = cam.scaledPixelWidth;

            var halfWidth =  width / 2;
            var halfHeight =  height/ 2;
            var xMin = width - halfWidth;
            var xMax = width + halfWidth;
            var yMin = height + halfHeight;
            var yMax = height + height;
            
            var spawnBounds = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            _placeAreas.Add(new Rectangle(spawnBounds));
        }
    }
}