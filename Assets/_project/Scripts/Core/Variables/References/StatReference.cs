using System;

namespace _project.Scripts.Core.Variables.References
{
    [Serializable]
    public class StatReference
    {
        public bool useConstant;
        public float constantValue;
        public StatVariable variable;

        public StatReference()
        { }

        public StatReference(float value)
        {
            useConstant = true;
            constantValue = value;
        }

        public float Value => useConstant ? constantValue : variable.value;

        public static implicit operator float(StatReference reference)
        {
            return reference.Value;
        }

        public static implicit operator int(StatReference reference)
        {
            return (int)reference.Value;
        }
    }
}