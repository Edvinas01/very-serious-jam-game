using System;
using System.Collections;
using System.Linq;
using InSun.GameCore.Pooling;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

// ReSharper disable HeuristicUnreachableCode
namespace InSun.GameCore.Tests.Runtime
{
    internal sealed class SimpleObjectPoolingSystemTests
    {
        private SimpleObjectPoolingSystem system;
        private Camera mainCamera;

        [SetUp]
        public void SetUp()
        {
            var gameObject = new GameObject("PoolingSystem");
            system = gameObject.AddComponent<SimpleObjectPoolingSystem>();

            var cameraGameObject = new GameObject("MainCamera")
            {
                tag = "MainCamera",
            };

            mainCamera = cameraGameObject.AddComponent<Camera>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(system.gameObject);
            Object.Destroy(mainCamera.gameObject);
        }

        [UnityTest]
        public IEnumerator Should_Preload()
        {
            //
            // Given: pool with a preload count
            //
            var objectPool = new TestObjectPool
            {
                PreloadCount = 3,
            };

            //
            // When: preloading the pool
            //
            system.Preload(objectPool);

            //
            // Then: objects preloaded on the system
            //
            Assert.IsTrue(system.IsPreloading);
            yield return new WaitWhile(() => system.IsPreloading);

            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));

            var pooledObjects = system.GetComponentsInChildren<PooledGameObject>(includeInactive: true);
            Assert.AreEqual(3, pooledObjects.Length);
            Assert.AreEqual(0, pooledObjects.Count(obj => obj.IsActive));
        }

        [UnityTest]
        public IEnumerator Should_GetInstance()
        {
            //
            // Given: pool with a preloaded object inside it
            //
            var objectPool = new TestObjectPool();

            //
            // When: retrieving an obj from the pool
            //
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: object should be retrieved
            //
            if (instance is not PooledGameObject typedInstance)
            {
                Assert.Fail($"{instance} is not a {typeof(PooledGameObject)}");
                yield break;
            }

            Assert.AreEqual(1, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsTrue(typedInstance.IsActive);
            Assert.IsTrue(typedInstance.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_And_Release()
        {
            //
            // Given: pool with a preloaded object inside it
            //
            var objectPool = new TestObjectPool();

            //
            // When: retrieving an obj from the pool and releasing it
            //
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            instance.Release();
            yield return null;

            //
            // Then: object should be retrieved and disposed correctly
            //
            if (instance is not PooledGameObject typedInstance)
            {
                Assert.Fail($"{instance} is not a {typeof(PooledGameObject)}");
                yield break;
            }

            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsFalse(typedInstance.IsActive);
            Assert.IsFalse(typedInstance.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_And_DisabledExternally()
        {
            //
            // Given: retrieved obj from the pool
            //
            var objectPool = new TestObjectPool();
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            if (instance is not PooledGameObject typedInstance)
            {
                Assert.Fail($"{instance} is not a {typeof(PooledGameObject)}");
                yield break;
            }

            //
            // When: object is disabled externally
            //
            typedInstance.gameObject.SetActive(false);
            yield return null;

            //
            // Then: object should be returned to pool
            //
            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsFalse(typedInstance.IsActive);
            Assert.IsFalse(typedInstance.gameObject.activeSelf);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_And_DestroyedExternally()
        {
            //
            // Given: retrieved obj from the pool
            //
            var objectPool = new TestObjectPool();
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            if (instance is not PooledGameObject typedInstance)
            {
                Assert.Fail($"{instance} is not a {typeof(PooledGameObject)}");
                yield break;
            }

            //
            // When: object is destroyed externally
            //
            Object.Destroy(typedInstance.gameObject);
            yield return null;

            //
            // Then: object should be removed from pool
            //
            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));
        }

        [UnityTest]
        public IEnumerator Should_NotGetInstance_MaxCountReached()
        {
            //
            // Given: pool at max load with stealing disabled
            //
            var objectPool = new TestObjectPool
            {
                MaxInstanceCount = 1,
            };

            var firstInstance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // When: get another instance
            //
            var secondInstance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: fail to get a second instance
            //
            Assert.AreEqual(1, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsTrue(firstInstance.IsActive);
            Assert.IsFalse(secondInstance.IsActive);
            Assert.IsTrue(secondInstance is DefaultPooledObject);
        }

        [UnityTest]
        public IEnumerator Should_StealInstance_MaxCountReached_And_Stealing()
        {
            //
            // Given: pool at max load with stealing enabled
            //
            var objectPool = new TestObjectPool
            {
                MaxInstanceCount = 1,
                IsStealing = true,
            };

            system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // When: get another while at max
            //
            var secondInstance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: first should be stolen and returned to pool, second should be active
            //
            Assert.AreEqual(1, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsTrue(secondInstance.IsActive);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_FrustumCull_InsideFrustum()
        {
            //
            // Given: culling enabled and camera behind the spawn point (camera sees the obj)
            //
            mainCamera.transform.position = new Vector3(0f, 0f, -10f);

            var objectPool = new TestObjectPool
            {
                IsFrustumCull = true,
                ObjectSize = 0f,
            };

            //
            // When: getting instance at a position in front of the cam
            //
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: instance should be fetched
            //
            Assert.IsTrue(instance.IsActive);
            Assert.IsFalse(instance is DefaultPooledObject);
        }

        [UnityTest]
        public IEnumerator Should_NotGetInstance_FrustumCull_OutsideFrustum()
        {
            //
            // Given: culling enabled and camera behind the spawn point (camera doesn't the obj)
            //
            var cameraGameObject = new GameObject("MainCamera")
            {
                tag = "MainCamera",
            };
            var camera = cameraGameObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(0f, 0f, -10f);

            var objectPool = new TestObjectPool
            {
                IsFrustumCull = true,
                ObjectSize = 0f,
            };

            //
            // When: getting instance at a position behind the camera
            //
            var instance = system.GetInstance(objectPool, new Vector3(0f, 0f, -20f), Quaternion.identity);
            yield return null;

            //
            // Then: instance should be fetched
            //
            Assert.IsFalse(instance.IsActive);
            Assert.IsTrue(instance is DefaultPooledObject);

            Object.Destroy(cameraGameObject);
        }

        [UnityTest]
        public IEnumerator Should_NotGetInstance_SpawnCooldown()
        {
            //
            // Given: pool with a cooldown, first instance already gotten
            //
            var objectPool = new TestObjectPool
            {
                SpawnCooldown = 10f,
            };

            var firstInstance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // When: requesting another instance before cooldown expires
            //
            var secondInstance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: second request should be denied by cooldown
            //
            Assert.IsTrue(firstInstance.IsActive);
            Assert.IsFalse(secondInstance.IsActive);
            Assert.IsTrue(secondInstance is DefaultPooledObject);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_SpawnChanceIsZero()
        {
            //
            // Given: pool with 100% spawn chance
            //
            var objectPool = new TestObjectPool
            {
                SpawnChance = 0,
            };

            //
            // When: get an instance
            //
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then: instance should never be spawned (gamba fail)
            //
            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsFalse(instance.IsActive);
            Assert.IsTrue(instance is DefaultPooledObject);
        }

        [UnityTest]
        public IEnumerator Should_GetInstance_Particles_And_Release_Automatically()
        {
            const float particleLifetime = 0.1f;

            //
            // Given: pool with a preloaded object inside it that has particles on it
            //
            var objectPool = new TestObjectPool();
            objectPool.OnCreated += obj =>
            {
                var particleSystem = obj.gameObject.AddComponent<ParticleSystem>();
                var mainModule = particleSystem.main;
                mainModule.duration = particleLifetime;
                mainModule.startLifetime = new ParticleSystem.MinMaxCurve(particleLifetime);
                mainModule.loop = false;

                obj.gameObject.AddComponent<PooledParticles>();
            };

            //
            // When: retrieving an obj from the pool and releasing it
            //
            var instance = system.GetInstance(objectPool, Vector3.zero, Quaternion.identity);
            yield return null;

            //
            // Then (1): object should be retrieved and disposed correctly
            //
            if (instance is not PooledGameObject typedInstance)
            {
                Assert.Fail($"{instance} is not a ${typeof(PooledGameObject)}");
                yield break;
            }

            var particles = typedInstance.GetComponent<PooledParticles>();
            if (particles == false)
            {
                Assert.Fail($"{typedInstance.name} does not have {typeof(PooledParticles)} comp");
            }

            Assert.AreEqual(1, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsTrue(typedInstance.IsActive);
            Assert.IsTrue(typedInstance.gameObject.activeSelf);
            Assert.IsTrue(particles.IsPlaying);

            //
            // Then (2): particles should auto dispose as they finished
            //
            yield return new WaitForSeconds(particleLifetime * 2f);

            Assert.AreEqual(0, system.GetActiveObjectCount(objectPool.PoolId));
            Assert.IsFalse(typedInstance.IsActive);
            Assert.IsFalse(typedInstance.gameObject.activeSelf);
            Assert.IsFalse(particles.IsPlaying);
        }

        private sealed class TestObjectPool : IObjectPool
        {
            public string Name => "BasicTestPool";

            public PoolId PoolId => new(1);

            public int MaxInstanceCount { get; set; } = 10;

            public int PreloadCount { get; set; }

            public float SpawnCooldown { get; set; }

            public float SpawnChance { get; set; } = 1f;

            public bool IsStealing { get; set; }

            public bool IsFrustumCull { get; set; }

            public float ObjectSize { get; set; }

            public event Action<PooledGameObject> OnCreated;

            public IPooledObject Create(Transform parent)
            {
                var testPooledObject = new GameObject("TestPooledObject");
                testPooledObject.transform.SetParent(parent, false);

                var testPooledGameObject = testPooledObject.AddComponent<PooledGameObject>();
                OnCreated?.Invoke(testPooledGameObject);

                return testPooledGameObject;
            }
        }
    }
}
