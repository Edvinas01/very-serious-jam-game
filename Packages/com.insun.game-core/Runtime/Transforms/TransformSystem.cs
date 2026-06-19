using System.Collections.Generic;
using InSun.GameCore.Objects;
using UnityEngine;

namespace InSun.GameCore.Transforms
{
    public sealed class TransformSystem : ILifecycleListener
    {
        private readonly IDictionary<string, Transform> transforms = new Dictionary<string, Transform>();

        public void OnInitialized()
        {
        }

        public void OnDisposed()
        {
        }

        public Transform GetTransform(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            if (transforms.TryGetValue(name, out var cachedTransform) && cachedTransform)
            {
                return cachedTransform;
            }

            var transformGameObject = new GameObject(name);
            var transform = transformGameObject.transform;
            transforms[name] = transform;

            return transform;
        }
    }
}
