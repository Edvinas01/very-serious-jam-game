using System;
using UnityEngine;

namespace InSun.GameCore.Pooling
{
    public interface IPooledObject
    {
        public PoolId PoolId { get; }

        public string Name { get; set; }

        public Transform Parent { get; set; }

        public Pose Pose { get; set; }

        public bool IsActive { get; set; }

        public event Action<IPooledObject> OnReleased;

        public event Action<IPooledObject> OnRetrieved;

        public void Release();

        public void Initialize(
            PoolId id,
            Action<IPooledObject> onReleaseToPoolCallback,
            Action<IPooledObject> onDestroyedCallback
        );

        public bool TryActivate();

        public void Dispose();

        public void Destroy();

        public bool TryGetComponent<T>(out T component);
    }
}
