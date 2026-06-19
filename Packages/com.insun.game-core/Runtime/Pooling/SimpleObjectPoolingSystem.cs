using System.Collections;
using System.Collections.Generic;
using InSun.GameCore.Objects;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace InSun.GameCore.Pooling
{
    public sealed class SimpleObjectPoolingSystem : MonoBehaviour, IObjectPoolingSystem, ILateUpdateListener
    {
        [Min(1)]
        [SerializeField]
        private int preloadBatchSize = 100;

        private readonly Dictionary<PoolId, ObjectPool<IPooledObject>> objectPoolsByPoolId = new();
        private readonly Dictionary<PoolId, HashSet<IPooledObject>> objectsByPoolId = new();
        private readonly Dictionary<PoolId, float> spawnTimeByPoolId = new();

        private readonly Plane[] mainCameraFrustumPlanes = new Plane[6];
        private bool isFrustumPlanesInitialized;
        private Camera mainCamera;

        private Transform pendingInstanceTransform;
        private int preloadingCount;

        public bool IsPreloading => preloadingCount > 0;

        private void Awake()
        {
            var pendingInstanceGameObject = new GameObject("Pool_PendingInstances")
            {
                transform = { parent = transform },
            };

            pendingInstanceGameObject.SetActive(false);
            pendingInstanceTransform = pendingInstanceGameObject.transform;
        }

        public void OnLateUpdated(float deltaTime)
        {
            isFrustumPlanesInitialized = false;
        }

        public void Preload(IEnumerable<IObjectPool> objectPools)
        {
            StartCoroutine(PreloadInstancesRoutine(objectPools));
        }

        public void Preload(IObjectPool objectPool)
        {
            if (objectPool.PreloadCount <= 0)
            {
                return;
            }

            StartCoroutine(PreloadInstancesRoutine(new[] { objectPool }));
        }

        public int GetActiveObjectCount(PoolId poolId)
        {
            return objectsByPoolId.TryGetValue(poolId, out var objects) ? objects.Count : 0;
        }

        public IPooledObject GetInstance(IObjectPool objectPool, Vector3 position, Quaternion rotation)
        {
            var poolId = objectPool.PoolId;

            if (objectPool.IsFrustumCull && IsWithinFrustum(position, objectPool.ObjectSize) == false)
            {
                return DefaultPooledObject.Instance;
            }

            if (objectPool.SpawnChance < 1f && (objectPool.SpawnChance <= 0f || Random.value < 1f - objectPool.SpawnChance))
            {
                return DefaultPooledObject.Instance;
            }

            if (objectPool.SpawnCooldown > 0f)
            {
                var currentTime = Time.time;
                if (spawnTimeByPoolId.TryGetValue(poolId, out var spawnTime) && currentTime - spawnTime < objectPool.SpawnCooldown)
                {
                    return DefaultPooledObject.Instance;
                }

                spawnTimeByPoolId[poolId] = currentTime;
            }

            if (objectsByPoolId.TryGetValue(poolId, out var objects) && objects.Count >= objectPool.MaxInstanceCount)
            {
                if (objectPool.IsStealing == false)
                {
                    return DefaultPooledObject.Instance;
                }

                using var enumerator = objects.GetEnumerator();
                if (enumerator.MoveNext() == false)
                {
                    return DefaultPooledObject.Instance;
                }

                var stolen = enumerator.Current;
                if (stolen == null)
                {
                    return DefaultPooledObject.Instance;
                }

                stolen.Release();
            }

            var instancePool = GetOrCreatePool(objectPool);
            var instance = instancePool.Get();

            if (instance.IsActive == false)
            {
                // Busted instance
                return DefaultPooledObject.Instance;
            }

            instance.Pose = new Pose(position, rotation);

            return instance;
        }

        private IEnumerator PreloadInstancesRoutine(IEnumerable<IObjectPool> objectPools)
        {
            preloadingCount++;

            try
            {
                foreach (var evt in objectPools)
                {
                    if (evt.PreloadCount > 0)
                    {
                        yield return PreloadInstanceRoutine(evt);
                    }
                }
            }
            finally
            {
                preloadingCount--;
            }

            yield break;

            IEnumerator PreloadInstanceRoutine(IObjectPool evt)
            {
                var count = evt.PreloadCount;
                var pool = GetOrCreatePool(evt);
                var instances = new List<IPooledObject>(count);

                for (var index = 0; index < count && pool.CountAll < count; index++)
                {
                    instances.Add(pool.Get());

                    if (index > 0 && index % preloadBatchSize == 0)
                    {
                        yield return null;
                    }
                }

                foreach (var instance in instances)
                {
                    pool.Release(instance);
                }
            }
        }

        private ObjectPool<IPooledObject> GetOrCreatePool(IObjectPool objectPool)
        {
            var poolId = objectPool.PoolId;
            if (objectPoolsByPoolId.TryGetValue(poolId, out var pool))
            {
                return pool;
            }

            var poolParentGameObject = new GameObject($"Pool - {objectPool.Name} ({objectPool.PoolId.Value})")
            {
                transform = { parent = transform },
            };

            var poolTransform = poolParentGameObject.transform;

            var newPool = new ObjectPool<IPooledObject>(
                createFunc: () =>
                {
                    var instance = objectPool.Create(pendingInstanceTransform);
                    instance.IsActive = false;
                    instance.Parent = poolTransform;

                    if (objectsByPoolId.TryGetValue(poolId, out var list))
                    {
                        instance.Name = $"{objectPool.Name} ({list.Count})";
                    }
                    else
                    {
                        instance.Name = $"{objectPool.Name}";
                    }

                    instance.Initialize(
                        id: objectPool.PoolId,
                        onReleaseToPoolCallback: OnReleaseToPool,
                        onDestroyedCallback: OnInstanceDestroyed
                    );

                    return instance;
                },
                actionOnGet: instance =>
                {
                    if (instance.TryActivate() == false)
                    {
                        // Failed to activate internall - invalid obj
                        return;
                    }

                    if (objectsByPoolId.TryGetValue(poolId, out var objects) == false)
                    {
                        objects = new HashSet<IPooledObject>();
                        objectsByPoolId[poolId] = objects;
                    }

                    objects.Add(instance);
                },
                actionOnRelease: instance =>
                {
                    if (objectsByPoolId.TryGetValue(poolId, out var objects))
                    {
                        objects.Remove(instance);
                    }

                    instance.Dispose();
                },
                actionOnDestroy: instance =>
                {
                    if (Game.IsQuitting)
                    {
                        return;
                    }

                    instance.Destroy();
                },
                collectionCheck: false
            );

            objectPoolsByPoolId[poolId] = newPool;

            return newPool;
        }

        private void OnReleaseToPool(IPooledObject obj)
        {
            if (objectPoolsByPoolId.TryGetValue(obj.PoolId, out var pool))
            {
                pool.Release(obj);
            }
        }

        private void OnInstanceDestroyed(IPooledObject obj)
        {
            if (objectsByPoolId.TryGetValue(obj.PoolId, out var set))
            {
                set.Remove(obj);
            }
        }

        private bool IsWithinFrustum(Vector3 position, float objectSize)
        {
            if (mainCamera == false)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera == false)
            {
                return false;
            }

            if (isFrustumPlanesInitialized == false)
            {
                GeometryUtility.CalculateFrustumPlanes(mainCamera, mainCameraFrustumPlanes);
                isFrustumPlanesInitialized = true;
            }

            return GeometryUtility.TestPlanesAABB(
                mainCameraFrustumPlanes,
                new Bounds(
                    center: position,
                    size: Vector3.one * objectSize
                )
            );
        }
    }
}
