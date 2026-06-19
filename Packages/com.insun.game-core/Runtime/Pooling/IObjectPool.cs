using UnityEngine;

namespace InSun.GameCore.Pooling
{
    public interface IObjectPool
    {
        public string Name { get; }

        public PoolId PoolId { get; }

        public int MaxInstanceCount { get; }

        public int PreloadCount { get; }

        public float SpawnCooldown { get; }

        public float SpawnChance { get; }

        public bool IsStealing { get; }

        public bool IsFrustumCull { get; }

        public float ObjectSize { get; }

        public IPooledObject Create(Transform parent);
    }
}
