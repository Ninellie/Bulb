using Scellecs.Morpeh;
using UnityEngine;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    /// <summary>
    /// Однокадровый компонент-событие, предназначенный для чтения другими системами.
    /// Создаётся и удаляется исключительно в BlocksToolBarPanelSystem.
    /// </summary>
    public struct BlockButtonClickEvent : IComponent
    {
        public string BlockTileName;
    }
}