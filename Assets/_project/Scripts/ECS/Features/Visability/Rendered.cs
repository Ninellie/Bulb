using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Visability
{
    [Serializable]
    public struct Rendered : IComponent
    {
        [field:SerializeField] public Renderer Renderer { get; set; }
    }
}