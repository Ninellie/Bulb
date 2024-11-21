using UnityEngine;
using UnityEngine.EventSystems;

namespace _project.Scripts.ECS.Features.TileReplacement
{
    public class CurrentBlockSelect : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string blockName;
        [SerializeField] private BlockDataPreset blockDataPreset;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            blockDataPreset.SetCurrentByName(blockName);
        }
    }
}