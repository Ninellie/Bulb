using _project.Scripts.Core.Variables.References;
using Core.Events;
using UnityEngine;

namespace EntityComponents.UnitComponents.EnemyComponents
{
    public class Dumper : MonoBehaviour
    {
        [SerializeField] private TransformPoolReference _drops;
        [field: SerializeField] public bool CanDrop { get; set; }
        [SerializeField] private GameEvent _onBonusDrop;

        private void OnEnable()
        {
            CanDrop = true;
        }

        public void DropBonus()
        {
            if (!CanDrop) return;
            if (_onBonusDrop != null)
            {
                _onBonusDrop.Raise();
            }
            var drop = _drops.Value.Get(); // TODO Поменять TransformPool на BoonDataPool или типа того
            drop.SetPositionAndRotation(transform.position, Quaternion.identity);
            drop.gameObject.GetComponent<TrailRenderer>().Clear();
        }
    }
}