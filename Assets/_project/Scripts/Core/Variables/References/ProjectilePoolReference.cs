using System;
using EntityComponents.UnitComponents.PlayerComponents;

namespace Core.Variables.References
{
    [Serializable]
    public class ProjectilePoolReference
    {
        public bool useConstant;
        public ProjectilePool constantValue;
        public ProjectilePoolVariable variable;

        public ProjectilePoolReference()
        { }

        public ProjectilePoolReference(ProjectilePool value)
        {
            useConstant = true;
            constantValue = value;
        }

        public ProjectilePool Value
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

        public static implicit operator ProjectilePool(ProjectilePoolReference reference)
        {
            return reference.Value;
        }
    }
}