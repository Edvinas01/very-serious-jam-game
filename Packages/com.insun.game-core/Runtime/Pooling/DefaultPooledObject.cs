using System;
using UnityEngine;

namespace InSun.GameCore.Pooling
{
    internal sealed class DefaultPooledObject : IPooledObject
    {
        public static readonly DefaultPooledObject Instance = new();

        public PoolId PoolId => default;

        public string Name
        {
            get => "Default";
            set { }
        }

        public Transform Parent
        {
            get => null;
            set { }
        }

        public Pose Pose
        {
            get => Pose.identity;
            set { }
        }

        public bool IsActive
        {
            get => false;
            set { }
        }

        public bool IsDestroyed => false;

        public event Action<IPooledObject> OnReleased
        {
            add { }
            remove { }
        }

        public event Action<IPooledObject> OnRetrieved
        {
            add { }
            remove { }
        }

        private DefaultPooledObject()
        {
        }

        public void Initialize(
            PoolId id,
            Action<IPooledObject> onReleaseToPoolCallback,
            Action<IPooledObject> onDestroyedCallback
        )
        {
        }

        public void Release()
        {
        }

        public bool TryActivate()
        {
            return false;
        }

        public void Dispose()
        {
        }

        public void Destroy()
        {
        }

        public bool TryGetComponent<T>(out T component)
        {
            component = default;
            return false;
        }
    }
}
