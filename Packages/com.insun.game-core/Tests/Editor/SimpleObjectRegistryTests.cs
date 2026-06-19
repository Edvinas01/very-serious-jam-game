using System.Collections.Generic;
using System.Linq;
using InSun.GameCore.Objects;
using NUnit.Framework;

namespace InSun.GameCore.Tests.Editor
{
    internal sealed class SimpleObjectRegistryTests
    {
        private SimpleObjectRegistry registry;

        [SetUp]
        public void SetUp()
        {
            registry = new SimpleObjectRegistry();
        }

        [Test]
        public void Should_GetObject()
        {
            //
            // Given: registered object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: fetching the object
            //
            var result = registry.Get<ITestObject>();

            //
            // Then: object should be fetched
            //
            Assert.AreSame(@object, result);
        }

        [Test]
        public void Should_GetObject_NotFound()
        {
            //
            // Given: empty registry
            //
            // ...

            //
            // Then: fetching should throw
            //
            Assert.Throws<KeyNotFoundException>(() => registry.Get<ITestObject>());
        }

        [Test]
        public void Should_GetObject_ById()
        {
            //
            // Given: an object registered with an id
            //
            var @object = new TestObject();
            registry.Add<int, ITestObject>(1, @object);

            //
            // When: fetching by id
            //
            var result = registry.Get<int, ITestObject>(1);

            //
            // Then: object should be fetched
            //
            Assert.AreSame(@object, result);
        }

        [Test]
        public void Should_GetObject_ById_NotFound()
        {
            //
            // Given: empty registry
            //
            // ...

            //
            // Then: fetching should throw
            //
            Assert.Throws<KeyNotFoundException>(() => registry.Get<int, ITestObject>(1));
        }

        [Test]
        public void Should_TryGetObject()
        {
            //
            // Given: registered object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // Then: object should be fetched
            //
            var isFound = registry.TryGet<ITestObject>(out var result);

            Assert.IsTrue(isFound);
            Assert.AreSame(@object, result);
        }

        [Test]
        public void Should_TryGetObject_NotFound()
        {
            //
            // Given: empty registry
            //
            // ...

            //
            // When: fetching an object
            //
            var isFound = registry.TryGet<ITestObject>(out var result);

            //
            // Then: object should not be found
            //
            Assert.IsFalse(isFound);
            Assert.IsNull(result);
        }

        [Test]
        public void Should_TryGetObject_ById()
        {
            //
            // Given: an object registered with an id
            //
            var @object = new TestObject();
            registry.Add<int, ITestObject>(1, @object);

            //
            // When: fetching by id
            //
            var isFound = registry.TryGet<int, ITestObject>(1, out var result);

            //
            // Then: object should be found
            //
            Assert.IsTrue(isFound);
            Assert.AreSame(@object, result);
        }

        [Test]
        public void Should_TryGetObject_ById_NotFound()
        {
            //
            // Given: empty registry
            //
            // ...

            //
            // When: fetching by id
            //
            var isFound = registry.TryGet<int, ITestObject>(1, out var result);

            //
            // Then: object should not be found
            //
            Assert.IsFalse(isFound);
            Assert.IsNull(result);
        }

        [Test]
        public void Should_GetAllObjects()
        {
            //
            // Given: registered object and two objects with ids
            //
            var objectA = new TestObject();
            var objectB = new TestObject();
            var objectC = new TestObject();

            registry.Add<ITestObject>(objectA);
            registry.Add<int, ITestObject>(1, objectB);
            registry.Add<int, ITestObject>(2, objectC);

            //
            // When: fetching all objects
            //
            var buffer = new List<ITestObject>();
            registry.GetAll(buffer);

            //
            // Then: all three should be fetched
            //
            Assert.AreEqual(3, buffer.Count);
            Assert.IsTrue(buffer.Contains(objectA));
            Assert.IsTrue(buffer.Contains(objectB));
            Assert.IsTrue(buffer.Contains(objectC));
        }

        [Test]
        public void Should_GetAllObjects_AfterRemove()
        {
            //
            // Given: three objects
            //
            var objectA = new TestObject();
            var objectB = new TestObject();
            var objectC = new TestObject();

            registry.Add<int, ITestObject>(1, objectA);
            registry.Add<int, ITestObject>(2, objectB);
            registry.Add<int, ITestObject>(3, objectC);

            //
            // When: removing the middle object
            //
            registry.Remove<int, ITestObject>(2);

            //
            // Then: two should remain and be still valid
            //
            var buffer = new List<ITestObject>();
            registry.GetAll(buffer);

            Assert.AreEqual(2, buffer.Count);
            Assert.IsTrue(buffer.Contains(objectA));
            Assert.IsTrue(buffer.Contains(objectC));
        }

        [Test]
        public void Should_Call_OnInitialized_RegistryInitialized()
        {
            //
            // Given: an object registered before the registry is initialized
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: initializing the registry
            //
            registry.Initialize();

            //
            // Then: object should receive OnInitialized
            //
            Assert.AreEqual(1, @object.InitializedCount);
            Assert.AreEqual(0, @object.DisposedCount);
        }

        [Test]
        public void Should_Call_OnInitialized_ObjectAdded()
        {
            //
            // Given: already initialized registry
            //
            registry.IsInitialized = true;

            //
            // When: adding an object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // Then: object should receive OnInitialized
            //
            Assert.AreEqual(1, @object.InitializedCount);
            Assert.AreEqual(0, @object.DisposedCount);
        }

        [Test]
        public void Should_Call_OnDisposed_ObjectRemoved()
        {
            //
            // Given: registered object (uninitialized)
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: removing the object
            //
            registry.Remove<ITestObject>();

            //
            // Then: object should receive OnDisposed
            //
            Assert.AreEqual(0, @object.InitializedCount);
            Assert.AreEqual(1, @object.DisposedCount);
        }

        [Test]
        public void Should_Call_OnDisposed_RegistryDisposed()
        {
            //
            // Given: registered object (uninitialized)
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: disposing registry
            //
            registry.Dispose();

            //
            // Then: object should receive OnDisposed
            //
            Assert.AreEqual(0, @object.InitializedCount);
            Assert.AreEqual(1, @object.DisposedCount);
        }


        [Test]
        public void Should_Call_OnFixedUpdated_FixedUpdate()
        {
            //
            // Given: registered update listener
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: fixed updating the registry
            //
            registry.FixedUpdate();

            //
            // Then: listener should be called
            //
            Assert.AreEqual(1, @object.FixedUpdateCount);
        }

        [Test]
        public void Should_Call_OnUpdated_Update()
        {
            //
            // Given: registered update listener
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: updating the registry
            //
            registry.Update();

            //
            // Then: listener should be called
            //
            Assert.AreEqual(1, @object.UpdateCount);
        }

        [Test]
        public void Should_Call_OnLateUpdated_LateUpdate()
        {
            //
            // Given: registered update listener
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: late updating the registry
            //
            registry.LateUpdate();

            //
            // Then: listener should be called
            //
            Assert.AreEqual(1, @object.LateUpdateCount);
        }

        [Test]
        public void Should_Invoke_OnObjectRegistered_ObjectAdded()
        {
            //
            // Given: listener on the event
            //
            object registeredObject = null;
            registry.OnObjectRegistered += obj => { registeredObject = obj; };

            //
            // When: adding an object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // Then: event should be raised
            //
            Assert.AreSame(@object, registeredObject);
        }

        [Test]
        public void Should_Invoke_OnObjectUnregistered_ObjectRemoved()
        {
            //
            // Given: registered object and listener on the event
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            object unregisteredObject = null;
            registry.OnObjectUnregistered += obj => { unregisteredObject = obj; };

            //
            // When: removing the object
            //
            registry.Remove<ITestObject>();

            //
            // Then: event should be raised
            //
            Assert.AreSame(@object, unregisteredObject);
        }

        [Test]
        public void Should_GetGroup()
        {
            //
            // Given: registered object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: getting a group
            //
            var group = registry.GetGroup<ITestObject>();

            //
            // Then: group should contain the object
            //
            Assert.AreEqual(1, group.Count);
            Assert.AreSame(@object, group[0]);
        }

        [Test]
        public void Should_GetGroup_Backfilled()
        {
            //
            // Given: objects already registered before group is created
            //
            var objectA = new TestObject();
            var objectB = new TestObject();

            registry.Add<int, ITestObject>(1, objectA);
            registry.Add<int, ITestObject>(2, objectB);

            //
            // When: getting a group after registration
            //
            var group = registry.GetGroup<ITestObject>();

            //
            // Then: group should contain both objects
            //
            Assert.AreEqual(2, group.Count);
            Assert.IsTrue(group.Contains(objectA));
            Assert.IsTrue(group.Contains(objectB));
        }

        [Test]
        public void Should_GetGroup_Cached()
        {
            //
            // Given: group already retrieved
            //
            var groupA = registry.GetGroup<ITestObject>();

            //
            // When: getting the same group again
            //
            var groupB = registry.GetGroup<ITestObject>();

            //
            // Then: same instance should be returned
            //
            Assert.AreSame(groupA, groupB);
        }

        [Test]
        public void Should_GetGroup_UpdatedOnAdd()
        {
            //
            // Given: existing group
            //
            var group = registry.GetGroup<ITestObject>();

            //
            // When: adding an object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // Then: group should reflect the addition
            //
            Assert.AreEqual(1, group.Count);
            Assert.AreSame(@object, group[0]);
        }

        [Test]
        public void Should_GetGroup_UpdatedOnRemove()
        {
            //
            // Given: existing group and a registered object
            //
            var group = registry.GetGroup<ITestObject>();

            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // When: removing the object
            //
            registry.Remove<ITestObject>();

            //
            // Then: group should be empty
            //
            Assert.AreEqual(0, group.Count);
        }

        [Test]
        public void Should_GetGroup_WithPredicate()
        {
            //
            // Given: two objects, one matching the predicate
            //
            var objectA = new TestObject { Tag = 1 };
            var objectB = new TestObject { Tag = 2 };

            registry.Add<int, ITestObject>(1, objectA);
            registry.Add<int, ITestObject>(2, objectB);

            //
            // When: creating group with a predicate
            //
            var group = registry.GetGroup<ITestObject>(obj => obj is TestObject { Tag: 1 });

            //
            // Then: only matching object
            //
            Assert.AreEqual(1, group.Count);
            Assert.AreSame(objectA, group[0]);
        }

        [Test]
        public void Should_GetGroup_WithPredicate_UpdatedOnAdd()
        {
            //
            // Given: predicate group with no objects
            //
            var group = registry.GetGroup<ITestObject>(obj => obj is TestObject { Tag: 1 });

            //
            // When: adding a matching and a non-matching object
            //
            var objectA = new TestObject { Tag = 1 };
            var objectB = new TestObject { Tag = 2 };

            registry.Add<int, ITestObject>(1, objectA);
            registry.Add<int, ITestObject>(2, objectB);

            //
            // Then: only matching object
            //
            Assert.AreEqual(1, group.Count);
            Assert.AreSame(objectA, group[0]);
        }

        [Test]
        public void Should_GetGroup_WithPredicate_UpdatedOnRemove()
        {
            //
            // Given: predicate group with a matching object
            //
            var @object = new TestObject { Tag = 1 };
            registry.Add<int, ITestObject>(1, @object);

            var group = registry.GetGroup<ITestObject>(obj => obj is TestObject { Tag: 1 });

            //
            // When: removing the object
            //
            registry.Remove<int, ITestObject>(1);

            //
            // Then: group should be empty
            //
            Assert.AreEqual(0, group.Count);
        }

        [Test]
        public void Should_Invoke_OnObjectAdded_GroupObjectAdded()
        {
            //
            // Given: group and a listener on the event
            //
            var group = registry.GetGroup<ITestObject>();

            ITestObject addedObject = null;
            group.OnObjectAdded += obj => { addedObject = obj; };

            //
            // When: adding an object
            //
            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            //
            // Then: event should be raised
            //
            Assert.AreSame(@object, addedObject);
        }

        [Test]
        public void Should_Invoke_OnObjectRemoved_GroupObjectRemoved()
        {
            //
            // Given: group with a registered object and a listener
            //
            var group = registry.GetGroup<ITestObject>();

            var @object = new TestObject();
            registry.Add<ITestObject>(@object);

            ITestObject removedObject = null;
            group.OnObjectRemoved += obj => { removedObject = obj; };

            //
            // When: removing the object
            //
            registry.Remove<ITestObject>();

            //
            // Then: event should be raised
            //
            Assert.AreSame(@object, removedObject);
        }

        private interface ITestObject
        {
        }

        private sealed class TestObject : ITestObject, ILifecycleListener, IFixedUpdateListener, IUpdateListener, ILateUpdateListener
        {
            public int Tag { get; set; }

            public int InitializedCount { get; private set; }

            public int DisposedCount { get; private set; }

            public int FixedUpdateCount { get; private set; }

            public int UpdateCount { get; private set; }

            public int LateUpdateCount { get; private set; }

            public void OnInitialized()
            {
                InitializedCount++;
            }

            public void OnDisposed()
            {
                DisposedCount++;
            }

            public void OnFixedUpdated(float deltaTime)
            {
                FixedUpdateCount++;
            }

            public void OnUpdated(float deltaTime)
            {
                UpdateCount++;
            }

            public void OnLateUpdated(float deltaTime)
            {
                LateUpdateCount++;
            }
        }
    }
}
