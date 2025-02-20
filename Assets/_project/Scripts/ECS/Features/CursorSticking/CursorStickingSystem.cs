using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine.InputSystem;

namespace _project.Scripts.ECS.Features.CursorSticking
{
    /// <summary>
    /// Обрабатывает сущности с компонентом OnCursor
    /// и устанавливает их позицию на позицию курсора
    /// </summary>
    public class CursorStickingSystem : UpdateSystem
    {
        private Stash<OnCursor> _onCursorStash;
        
        public override void OnAwake()
        {
            _onCursorStash = World.GetStash<OnCursor>();
        }

        public override void OnUpdate(float deltaTime)
        {
            var pointerPos = Pointer.current.position.value; 
            
            foreach (var onCursor in _onCursorStash)
            {
                onCursor.Transform.position = pointerPos;
            }
        }
    }
}