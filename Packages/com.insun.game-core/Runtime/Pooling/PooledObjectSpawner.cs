using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Pooling
{
    public sealed class PooledObjectSpawner : MonoBehaviour
    {
        private enum UnityEventType
        {
            None = 0,
            Awake = 1,
            OnEnable = 2,
            Start = 3,
            OnDisable = 4,
            OnDestroy = 5,
        }

        [Header("General")]
        [SerializeField]
        private ObjectPoolData objectPool;

        [Header("Transforms")]
        [SerializeField]
        private bool isSetPosition = true;

        [SerializeField]
        private bool isSetRotation = true;

        [Header("Features")]
        [SerializeField]
        private UnityEventType preloadMode = UnityEventType.None;

        [SerializeField]
        private UnityEventType spawnMode = UnityEventType.None;

        [SerializeField]
        private UnityEventType releaseMode = UnityEventType.OnDisable;

        private readonly HashSet<IPooledObject> activeInstances = new();
        private IObjectPoolingSystem poolingSystem;

        public IReadOnlyCollection<IPooledObject> ActiveInstances => activeInstances;

        private void Awake()
        {
            poolingSystem = Game.GetObject<IObjectPoolingSystem>();

            AutoPreload(UnityEventType.Awake);
            AutoSpawn(UnityEventType.Awake);
            AutoRelease(UnityEventType.Awake);
        }

        private void Start()
        {
            AutoPreload(UnityEventType.Start);
            AutoSpawn(UnityEventType.Start);
            AutoRelease(UnityEventType.Start);
        }

        private void OnEnable()
        {
            AutoPreload(UnityEventType.OnEnable);
            AutoSpawn(UnityEventType.OnEnable);
            AutoRelease(UnityEventType.OnEnable);
        }

        private void OnDisable()
        {
            AutoPreload(UnityEventType.OnDisable);
            AutoSpawn(UnityEventType.OnDisable);
            AutoRelease(UnityEventType.OnDisable);
        }

        private void OnDestroy()
        {
            AutoPreload(UnityEventType.OnDestroy);
            AutoSpawn(UnityEventType.OnDestroy);
            AutoRelease(UnityEventType.OnDestroy);
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            SpawnInstance();
        }

        public IPooledObject SpawnInstance()
        {
            if (objectPool == false || isActiveAndEnabled == false)
            {
                return DefaultPooledObject.Instance;
            }

            return SpawnInternal(
                isSetPosition ? transform.position : Vector3.zero,
                isSetRotation ? transform.rotation : Quaternion.identity
            );
        }

        public IPooledObject SpawnInstance(Vector3 position, Quaternion rotation)
        {
            if (objectPool == false || isActiveAndEnabled == false)
            {
                return DefaultPooledObject.Instance;
            }

            return SpawnInternal(position, rotation);
        }

        public bool TrySpawnInstance(out IPooledObject instance)
        {
            return TrySpawnInstance(transform.position, transform.rotation, out instance);
        }

        public bool TrySpawnInstance(Vector3 position, Quaternion rotation, out IPooledObject instance)
        {
            if (objectPool == false || isActiveAndEnabled == false)
            {
                instance = null;
                return false;
            }

            instance = SpawnInternal(position, rotation);
            return instance.IsActive;
        }

        [ContextMenu("Release All")]
        public void ReleaseAll()
        {
            foreach (var instance in activeInstances)
            {
                instance.OnReleased -= OnInstanceReleased;
                instance.Release();
            }

            activeInstances.Clear();
        }

        private void AutoPreload(UnityEventType eventType)
        {
            if (preloadMode == eventType && objectPool)
            {
                poolingSystem.Preload(objectPool);
            }
        }

        private void AutoSpawn(UnityEventType eventType)
        {
            if (spawnMode == eventType && objectPool)
            {
                SpawnInternal(
                    isSetPosition ? transform.position : Vector3.zero,
                    isSetRotation ? transform.rotation : Quaternion.identity
                );
            }
        }

        private void AutoRelease(UnityEventType eventType)
        {
            if (releaseMode == eventType)
            {
                ReleaseAll();
            }
        }

        private IPooledObject SpawnInternal(Vector3 position, Quaternion rotation)
        {
            var instance = poolingSystem.GetInstance(objectPool, position, rotation);
            if (instance.IsActive == false)
            {
                return instance;
            }

            activeInstances.Add(instance);
            instance.OnReleased += OnInstanceReleased;

            return instance;
        }

        private void OnInstanceReleased(IPooledObject instance)
        {
            activeInstances.Remove(instance);
            instance.OnReleased -= OnInstanceReleased;
        }
    }
}
