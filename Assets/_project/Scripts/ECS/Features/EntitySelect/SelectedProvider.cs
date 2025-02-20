using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using UnityEngine.EventSystems;

namespace _project.Scripts.ECS.Features.EntitySelect
{
    public class SelectedProvider : MonoProvider<Selected>, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            var entity = World.Default.CreateEntity();
            ref var onSelect = ref entity.AddComponent<SelectRequest>();
            onSelect.SelectedEntity = Entity;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var entity = World.Default.CreateEntity();
            ref var onSelect = ref entity.AddComponent<DeselectRequest>();
            onSelect.DeselectedEntity = Entity;
        }
    }
}