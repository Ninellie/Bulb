using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine.EventSystems;

namespace _project.Scripts.ECS.Features.BlocksToolbarPanel
{
    public class BlockButtonProvider : MonoProvider<BlockButton>, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (Entity.Has<ButtonClicked>())
            {
                return;
            }
            Entity.AddComponent<ButtonClicked>();
        }
    }
}