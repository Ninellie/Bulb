using System;
using UnityEngine;

namespace Core.Variables.References
{
    [Serializable]
    public abstract class VariableReference<T>
    {
        [SerializeField] private bool useConstant;
        [SerializeField] private T constantValue;
        [SerializeField] private Variable<T> variable;

        protected VariableReference()
        { }

        protected VariableReference(T value)
        {
            useConstant = true;
            constantValue = value;
        }
        
        public T Value
        {
            get => useConstant ? constantValue : variable.Value;
            set
            {
                if (useConstant)
                {
                    constantValue = value;
                    return;
                }
                variable.Value = value;
            }
        }

        public static implicit operator T(VariableReference<T> reference)
        {
            return reference.Value;
        }
    }
    
    public abstract class Variable<T> : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developerDescription = "";
#endif
        
        [SerializeField] private T value;
        
        public T Value
        {
            get => value;
            set => this.value = value;
        }
        
        public void SetValue(T value)
        {
            this.value = value;
        }

        public void SetValue(Variable<T> value)
        {
            this.value = value.value;
        }
    }
}