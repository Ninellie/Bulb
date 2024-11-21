using EntityComponents.UnitComponents.PlayerComponents;
using UnityEngine;

namespace _project.Scripts.Core.Variables.Components
{
    public class TransformPoolSetter : MonoBehaviour
    {
        [SerializeField] private TransformPoolVariable _variable;

        private void Awake()
        {
            _variable.value = GetComponent<TransformPool>();
        }
    }
}