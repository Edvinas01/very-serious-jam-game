using System;

namespace InSun.GameCore.Entities
{
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public uint Value { get; }

        public ushort Generation { get; }

        public EntityId(uint value, ushort generation)
        {
            Value = value;
            Generation = generation;
        }

        public bool Equals(EntityId other)
        {
            return Value == other.Value && Generation == other.Generation;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Generation);
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return !a.Equals(b);
        }

        public override string ToString() => $"{Value}:{Generation}";
    }
}
