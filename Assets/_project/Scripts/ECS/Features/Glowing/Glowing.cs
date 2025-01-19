using System;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _project.Scripts.ECS.Features.Glowing
{
    [Serializable]
    public struct Glowing : IComponent
    {
        [field: SerializeField] public Light2D Light2D { get; set; } 
    }
}