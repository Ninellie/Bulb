using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    [Serializable]
    public struct BlockButton : IComponent
    {
        public string BlockTileName;
    }
}