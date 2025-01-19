using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    public struct BlockButtonClickEvent : IComponent
    {
        [field: SerializeField] public string BlockTileName { get; set; }
    }
}