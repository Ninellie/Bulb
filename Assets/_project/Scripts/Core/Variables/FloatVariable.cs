using UnityEngine;

namespace _project.Scripts.Core.Variables
{
    [CreateAssetMenu(fileName = "New Float Variable", menuName = "Variables/Float", order = 51)]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        public float value;

        public void SetValue(float value)
        {
            this.value = value;
        }

        public void SetValue(FloatVariable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(float amount)
        {
            value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            value += amount.value;
        }
        
        public static implicit operator float(FloatVariable reference)
        {
            return reference.value;
        }
        
        public static implicit operator int(FloatVariable reference)
        {
            return (int)reference.value;
        }
    }
}