using Scellecs.Morpeh;
using UnityEngine;
using System;
namespace _project.Scripts.ECS.Features.CursorSticking
{
    /// <summary>
    /// Сущности с этим компонентом будут прилипать к курсору
    /// </summary>
    [Serializable]
    public struct OnCursor : IComponent
    {
        public Transform Transform;
    }
}