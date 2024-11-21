using UnityEngine;

namespace _project.Scripts.Core.Variables
{
    [CreateAssetMenu(fileName = "Vector3Int Variable", menuName = "Variables/Vector3Int", order = 51)]
    public class Vector3IntVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public Vector3Int value;

        public void SetValue(Vector3Int value)
        {
            this.value = value;
        }

        public void SetValue(Vector3IntVariable value)
        {
            this.value = value.value;
        }
        
        public static implicit operator Vector3Int(Vector3IntVariable variable)
        {
            return variable.value;
        }
    }
}