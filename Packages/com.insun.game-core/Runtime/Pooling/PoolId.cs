using System;

namespace InSun.GameCore.Pooling
{
    public readonly struct PoolId : IEquatable<PoolId>
    {
        public int Value { get; }

        public PoolId(int value)
        {
            Value = value;
        }

        public bool Equals(PoolId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is PoolId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value;
        }
    }
}
