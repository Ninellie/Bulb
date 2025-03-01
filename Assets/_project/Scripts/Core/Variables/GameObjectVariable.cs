using UnityEngine;

namespace _project.Scripts.Core.Variables
{
    [CreateAssetMenu(fileName = "New GameObject Variable", menuName = "Variables/GameObject", order = 51)]
    public class GameObjectVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public GameObject value;

        public void SetValue(GameObject value)
        {
            this.value = value;
        }

        public void SetValue(GameObjectVariable value)
        {
            this.value = value.value;
        }
    }
}