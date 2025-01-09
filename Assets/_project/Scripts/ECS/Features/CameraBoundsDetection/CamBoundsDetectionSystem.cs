﻿using System.Collections.Generic;
using System.Linq;
using _project.Scripts.ECS.Features.Visability;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace _project.Scripts.ECS.Features.CameraBoundsDetection
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(CamBoundsDetectionSystem))]
    public sealed class CamBoundsDetectionSystem : UpdateSystem
    {
        private Stash<Rendered> _rendered;
        
        private Camera _mainCamera;

        private Filter _inCamBounds;
        private Filter _outCamBounds;
        
        public override void OnAwake()
        {
            _mainCamera = Camera.main;

            _rendered = World.GetStash<Rendered>();

            _inCamBounds = World.Filter.With<Rendered>().With<InMainCamBounds>().Build();
            _outCamBounds = World.Filter.With<Rendered>().Without<InMainCamBounds>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            var inCamBoundsRenderersTemp =
                GetCamBoundsRenderers(_inCamBounds);
            
            var outCamBoundsRenderersTemp = 
                GetCamBoundsRenderers(_outCamBounds);

#if UNITY_EDITOR
            Debug.Log($"Рендереров видно на камере: {inCamBoundsRenderersTemp.Count().ToString()}");
            Debug.Log($"Рендереров не видно на камере: {outCamBoundsRenderersTemp.Count().ToString()}");
#endif
                
            
            foreach (var (entity, renderer) in inCamBoundsRenderersTemp)
            {
                if (IsVisibleFromCamera(renderer)) continue;
                entity.RemoveComponent<InMainCamBounds>();
            }
            
            foreach (var (entity, renderer) in outCamBoundsRenderersTemp)
            {
                if (!IsVisibleFromCamera(renderer)) continue;
                entity.AddComponent<InMainCamBounds>();
            }
        }

        private IEnumerable<(Entity, Renderer)> GetCamBoundsRenderers(Filter camBoundsFilter)
        {
            var outCamBoundsTemp = new List<(Entity, Renderer)>();
            
            foreach (var entity in camBoundsFilter)
            {
                if (!TryGetRenderer(entity, out var renderer)) continue;
                outCamBoundsTemp.Add((entity, renderer));
            }

            return outCamBoundsTemp;
        }

        private bool TryGetRenderer(Entity entity, out Renderer renderer)
        {
            renderer = null;
            if (entity.IsNullOrDisposed()) return false;
            var rendered = _rendered.Get(entity);
            renderer = rendered.Renderer;
            return renderer.enabled;
        }
        

        private bool IsVisibleFromCamera(Renderer renderer)
        {
            // Получаем границы объекта (Bounds)
            var bounds = renderer.bounds;

            // Преобразуем границы камеры в мировые координаты
            var planes = GeometryUtility.CalculateFrustumPlanes(_mainCamera);

            // Проверяем, пересекаются ли границы объекта с границами камеры
            return GeometryUtility.TestPlanesAABB(planes, bounds);
        }
    }
}