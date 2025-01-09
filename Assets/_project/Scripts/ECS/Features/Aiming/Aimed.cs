using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Aiming
{
    [Serializable]
    public struct Aimed : IComponent
    {
        [field: SerializeField] public Transform Target { get; set; }
    }
}