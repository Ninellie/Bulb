using EntityComponents.UnitComponents.PlayerComponents;
using UnityEngine;

namespace _project.Scripts.Core.Variables.Components
{
    public class ProjectilePoolSetter : MonoBehaviour
    {
        [SerializeField] private ProjectilePoolVariable _variable;

        private void Awake()
        {
            _variable.value = GetComponent<ProjectilePool>();
        }
    }
}