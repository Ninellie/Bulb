using System;
using System.Collections.Generic;
using Core.Shapes;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using PixelPerfectCamera = UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

namespace _project.Scripts.ECS.Features.RandomPlacing
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

            var pixelCam = cam.GetComponent<PixelPerfectCamera>();
            var height = pixelCam.refResolutionY;
            var width = pixelCam.refResolutionX;

            var halfHeight =  height/ 2;
            var xMin = 0 - width;
            var xMax = 0 + width;
            var yMin = height + halfHeight;
            var yMax = height + height;
            
            var spawnBounds = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            _placeAreas.Add(new Rectangle(spawnBounds));
        }
    }
}