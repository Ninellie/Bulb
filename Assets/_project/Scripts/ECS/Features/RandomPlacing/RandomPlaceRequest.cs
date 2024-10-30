﻿using System;
using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.RandomPlacing
{
    [Serializable]
    public struct RandomPlaceRequest : IComponent
    {
        public Transform ObjectTransform { get; set; }
    }
}