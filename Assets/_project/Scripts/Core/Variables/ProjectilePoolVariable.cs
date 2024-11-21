using EntityComponents.UnitComponents.PlayerComponents;
using UnityEngine;

namespace _project.Scripts.Core.Variables
{
    [CreateAssetMenu(fileName = "New Projectile Pool Variable", menuName = "Variables/Projectile Pool", order = 51)]
    public class ProjectilePoolVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public ProjectilePool value;

        public void SetValue(ProjectilePool value)
        {
            this.value = value;
        }

        public void SetValue(ProjectilePoolVariable value)
        {
            this.value = value.value;
        }
    }
}