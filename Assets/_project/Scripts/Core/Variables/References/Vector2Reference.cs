using System;
using UnityEngine;

namespace _project.Scripts.Core.Variables.References
{
    [Serializable]
    public class Vector2Reference
    {
        public bool useConstant;
        public Vector2 constantValue;
        public Vector2Variable variable;

        public Vector2Reference()
        { }

        public Vector2Reference(Vector2 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector2 Value
        {
            get => useConstant ? constantValue : variable.value;

            set
            {
                if (useConstant)
                {
                    constantValue = value;
                }
                else
                {
                    variable.value = value;
                }
                
            }
        }

        public static implicit operator Vector2(Vector2Reference reference)
        {
            return reference.Value;
        }
    }
}