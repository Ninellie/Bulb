using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine.InputSystem;
using UnityEngine;

namespace _project.Scripts.ECS.Features.CursorSticking
{
    /// <summary>
    /// Обрабатывает сущности с компонентом OnCursor
    /// и устанавливает их позицию на позицию курсора
    /// </summary>
    [CreateAssetMenu(menuName = "ECS/Systems/Update/" + nameof(CursorStickingSystem))]
    public sealed class CursorStickingSystem : UpdateSystem
    {
        private Stash<OnCursor> _onCursorStash;
        private Camera _camera;

        public override void OnAwake()
        {
            _onCursorStash = World.GetStash<OnCursor>();
            _camera = Camera.main;
        }

        public override void OnUpdate(float deltaTime)
        {
            var pointerPos = Pointer.current.position.value;
            var pointerPosInWorld = _camera.ScreenToWorldPoint(pointerPos);
            foreach (var onCursor in _onCursorStash)
            {
                onCursor.Transform.position = new Vector3(pointerPosInWorld.x, pointerPosInWorld.y, 0);
            }
        }
    }
}