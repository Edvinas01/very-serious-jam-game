using System;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Pooling
{
    public sealed class PooledGameObject : MonoBehaviour, IPooledObject
    {
        [Header("Events")]
        [SerializeField]
        private bool isTriggerUnityEvents = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isTriggerUnityEvents))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTriggerUnityEvents))]
#endif
        [SerializeField]
        private UnityEvent onRetrievedFromPool;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isTriggerUnityEvents))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTriggerUnityEvents))]
#endif
        [SerializeField]
        private UnityEvent onReleasedToPool;

        private Action<IPooledObject> onReleaseToPool;
        private Action<IPooledObject> onDestroyed;

        private bool isObjectActive;
        private bool isInitialized;
        private bool isDestroyed;

        private Transform initialParent;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        public bool IsActive
        {
            get => isObjectActive;
            set
            {
                isObjectActive = value;
                gameObject.SetActive(value);
            }
        }

        public PoolId PoolId { get; private set; }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Transform Parent
        {
            get => transform.parent;
            set => transform.parent = value;
        }

        public Pose Pose
        {
            get => new(transform.position, transform.rotation);
            set => transform.SetPositionAndRotation(value.position, value.rotation);
        }

        public event Action<IPooledObject> OnReleased;

        public event Action<IPooledObject> OnRetrieved;

        private void Awake()
        {
            isObjectActive = gameObject.activeInHierarchy;
        }

        private void OnDisable()
        {
            if (isObjectActive)
            {
                Release();
            }
        }

        private void OnDestroy()
        {
            isDestroyed = true;

            if (isObjectActive)
            {
                isObjectActive = false;
                OnReleased?.Invoke(this);
            }

            if (isInitialized)
            {
                onDestroyed?.Invoke(this);
            }

            onReleaseToPool = null;
            onDestroyed = null;
            isInitialized = false;
        }

        public void Initialize(
            PoolId id,
            Action<IPooledObject> onReleaseToPoolCallback,
            Action<IPooledObject> onDestroyedCallback
        )
        {
            if (isInitialized)
            {
                Debug.LogWarning($"{name} is already initialized", this);
                return;
            }

            PoolId = id;

            initialParent = transform.parent;
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialScale = transform.localScale;

            onReleaseToPool = onReleaseToPoolCallback;
            onDestroyed = onDestroyedCallback;

            isInitialized = true;
        }

        public void Release()
        {
            if (isInitialized == false)
            {
                Debug.LogWarning($"{name} is not initialized", this);
                return;
            }

            if (isObjectActive == false)
            {
                Debug.LogWarning($"{name} is already returned to pool", this);
                return;
            }

            onReleaseToPool?.Invoke(this);
        }

        public bool TryActivate()
        {
            if (isInitialized == false)
            {
                Debug.LogWarning($"{name} is not initialized", this);
                return false;
            }

            if (isObjectActive)
            {
                Debug.LogWarning($"{name} is already retrieved from pool", this);
                return false;
            }

            if (isDestroyed)
            {
                return false;
            }

            transform.parent = initialParent;
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            transform.localScale = initialScale;

            isObjectActive = true;
            gameObject.SetActive(true);

            if (isObjectActive == false)
            {
                // When object was activating something yucky happened
                return false;
            }

            OnRetrieved?.Invoke(this);

            if (isTriggerUnityEvents)
            {
                onRetrievedFromPool?.Invoke();
            }

            return true;
        }

        public void Dispose()
        {
            if (isInitialized == false)
            {
                Debug.LogWarning($"{name} is not initialized", this);
                return;
            }

            if (isObjectActive == false)
            {
                Debug.LogWarning($"{name} is already returned to pool", this);
                return;
            }

            isObjectActive = false;
            gameObject.SetActive(false);

            OnReleased?.Invoke(this);

            if (isTriggerUnityEvents)
            {
                onReleasedToPool?.Invoke();
            }
        }

        public void Destroy()
        {
            if (isDestroyed)
            {
                return;
            }

            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
