using System;
using Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [Serializable]
    public struct Movable : IComponent
    {
        [SerializeField] private Vector2Reference targetPosition;
        [SerializeField] private FloatReference speed;
        [SerializeField] private Transform transform;

        public Transform Transform { get => transform; set => transform = value; }
        public Vector2Reference TargetPosition { get => targetPosition; set => targetPosition = value; }
        public FloatReference Speed { get => speed; set => speed = value; }
    }
}