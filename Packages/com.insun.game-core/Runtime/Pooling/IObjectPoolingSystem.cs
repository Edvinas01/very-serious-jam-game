using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Pooling
{
    public interface IObjectPoolingSystem
    {
        /// <summary>
        /// <c>true</c> if the pool is preloading objects currently or <c>false</c> otherwise.
        /// </summary>
        public bool IsPreloading { get; }

        /// <summary>
        /// Preload given pools with in-active objects.
        /// </summary>
        public void Preload(IEnumerable<IObjectPool> objectPools);

        /// <summary>
        /// Preload given pool with in-active objects.
        /// </summary>
        public void Preload(IObjectPool objectPool);

        /// <summary>
        /// Number of currently active objects in the given pool.
        /// </summary>
        public int GetActiveObjectCount(PoolId poolId);

        /// <returns>
        /// Pooled object instance - never <c>null</c>.
        /// </returns>
        public IPooledObject GetInstance(
            IObjectPool objectPool,
            Vector3 position = default,
            Quaternion rotation = default
        );
    }
}
