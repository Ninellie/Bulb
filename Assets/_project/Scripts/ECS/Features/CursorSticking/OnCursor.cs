using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.CursorSticking
{
    /// <summary>
    /// Сущности с этим компонентом будут прилипать к курсору
    /// </summary>
    public struct OnCursor : IComponent
    {
        public Transform Transform;
    }
}