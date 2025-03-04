using System;

namespace _project.Scripts.ECS.Features.Stats
{
    [Serializable]
    public struct StatMod : IEquatable<StatMod>
    {
        public StatModType Type;
        public float Value;
        
        [Serializable]
        public enum StatModType
        {
            Flat, Percentage
        }

        public bool Equals(StatMod other)
        {
            return Type == other.Type && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is StatMod other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Type, Value);
        }
    }
}