using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.RectSelection
{
    public struct SelectBoundsEvent : IComponent
    {
        public Bounds SelectionBounds { get; set; }
    }
}