using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    public struct BlockButton : IComponent
    {
        //public int Cost { get; set; } 
        [field: SerializeField] public string BlockTileName { get; set; } 
    }
}