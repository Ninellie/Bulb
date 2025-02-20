using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine.EventSystems;

namespace _project.Scripts.ECS.Features.EntitySelect
{
    public class SelectedProvider : MonoProvider<Selectable>, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            Entity.AddComponent<SelfSelectRequest>();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Entity.AddComponent<SelfDeselectRequest>();
        }
    }
}