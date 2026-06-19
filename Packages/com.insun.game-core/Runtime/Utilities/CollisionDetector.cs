using System;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    public sealed class CollisionDetector : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onCollisionEntered = new();

        [SerializeField]
        private UnityEvent onCollisionExited = new();

        public event Action<Collision> OnCollisionEntered;
        public event Action<Collision> OnCollisionExited;

        private void OnCollisionEnter(Collision other)
        {
            OnCollisionEntered?.Invoke(other);
            onCollisionEntered.Invoke();
        }

        private void OnCollisionExit(Collision other)
        {
            OnCollisionExited?.Invoke(other);
            onCollisionExited.Invoke();
        }
    }
}
