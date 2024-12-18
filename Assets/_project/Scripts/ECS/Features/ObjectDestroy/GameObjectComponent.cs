﻿using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.ObjectDestroy
{
    [Serializable]
    public struct GameObjectComponent : IComponent
    {
        [SerializeField] private GameObject gameObject;

        public GameObject GameObject
        {
            get => gameObject;
            set => gameObject = value;
        }
    }
}