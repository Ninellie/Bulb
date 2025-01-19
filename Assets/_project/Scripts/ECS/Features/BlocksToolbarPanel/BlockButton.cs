using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    public struct BlockButton : IComponent
    {
        //public int Cost { get; set; } 
        [field: SerializeField] public string BlockTileName { get; set; } 
    }
}