using System;
using _project.Scripts.Core.Variables.References;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.Movement
{
    [Serializable]
    public struct Movable : IComponent
    {
        [SerializeField] private Transform transform;
        [SerializeField] private bool directionAsTarget;
        [SerializeField] private Vector2Reference direction;
        [SerializeField] private FloatReference speed;
        [SerializeField] private float speedScale;

        public Transform Transform { get => transform; set => transform = value; }
        public bool DirectionAsTarget { get => directionAsTarget; set => directionAsTarget = value; }
        public Vector2Reference Direction { get => direction; set => direction = value; }
        public FloatReference Speed { get => speed; set => speed = value; }
        public float SpeedScale { get => speedScale; set => speedScale = value; }
    }
}