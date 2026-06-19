using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InSun.GameCore.Objects
{
    internal sealed class SimpleObjectRegistry : IObjectRegistry
    {
        private static readonly List<Type> PendingRemoveTypesBuffer = new();

        private readonly Dictionary<Type, ObjectPool> poolsByType = new();
        private readonly Dictionary<Type, object> objectsByType = new();
        private readonly Dictionary<object, int> bindingCountsByObject = new();
        private readonly List<object> allObjects = new();
        private readonly Dictionary<object, int> allObjectIndices = new();

        private readonly Dictionary<Type, ObjectGroup> groupsByType = new();
        private readonly Dictionary<PredicateKey, ObjectGroup> groupsByPredicate = new();
        private readonly List<ObjectGroup> allGroups = new();

        private readonly ObjectGroup<IFixedUpdateListener> fixedUpdateGroup;
        private readonly ObjectGroup<IUpdateListener> updateGroup;
        private readonly ObjectGroup<ILateUpdateListener> lateUpdateGroup;

        public bool IsInitialized { private get; set; }

        public event Action<object> OnObjectRegistered;

        public event Action<object> OnObjectUnregistered;

        public SimpleObjectRegistry()
        {
            fixedUpdateGroup = CreateGroup<IFixedUpdateListener>();
            updateGroup = CreateGroup<IUpdateListener>();
            lateUpdateGroup = CreateGroup<ILateUpdateListener>();
        }

        public void Initialize()
        {
            foreach (var obj in allObjects)
            {
                if (obj is ILifecycleListener lifecycleAware)
                {
                    lifecycleAware.OnInitialized();
                }
            }
        }

        public void Dispose()
        {
            foreach (var obj in allObjects)
            {
                if (obj is ILifecycleListener lifecycleAware)
                {
                    lifecycleAware.OnDisposed();
                }
            }
        }

        public void FixedUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (var index = 0; index < fixedUpdateGroup.Count; index++)
            {
                var listener = fixedUpdateGroup[index];
                try
                {
                    listener.OnFixedUpdated(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, listener as Object);
                }
            }
        }

        public void Update()
        {
            var deltaTime = Time.deltaTime;
            for (var index = 0; index < updateGroup.Count; index++)
            {
                var listener = updateGroup[index];
                try
                {
                    listener.OnUpdated(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, listener as Object);
                }
            }
        }

        public void LateUpdate()
        {
            var deltaTime = Time.deltaTime;
            for (var index = 0; index < lateUpdateGroup.Count; index++)
            {
                var listener = lateUpdateGroup[index];
                try
                {
                    listener.OnLateUpdated(deltaTime);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, listener as Object);
                }
            }
        }

        public IObjectGroup<TObject> GetGroup<TObject>() where TObject : class
        {
            var type = typeof(TObject);
            if (groupsByType.TryGetValue(type, out var existing))
            {
                return (ObjectGroup<TObject>)existing;
            }

            return CreateGroup<TObject>();
        }

        public IObjectGroup<TObject> GetGroup<TObject>(Func<TObject, bool> predicate) where TObject : class
        {
            var type = typeof(TObject);
            var key = new PredicateKey(predicate, type);
            if (groupsByPredicate.TryGetValue(key, out var existing))
            {
                return (ObjectGroup<TObject>)existing;
            }

            return CreateGroup(predicate);
        }

        public TObject Get<TObject>() where TObject : class
        {
            var type = typeof(TObject);
            if (objectsByType.TryGetValue(type, out var obj))
            {
                return (TObject)obj;
            }

            if (poolsByType.TryGetValue(type, out var pool) && pool.Count > 0)
            {
                return (TObject)pool.First;
            }

            throw new KeyNotFoundException($"Object of type {type.Name} was not found");
        }

        public bool TryGet<TObject>(out TObject obj) where TObject : class
        {
            var type = typeof(TObject);
            if (objectsByType.TryGetValue(type, out var raw))
            {
                obj = (TObject)raw;
                return true;
            }

            if (poolsByType.TryGetValue(type, out var pool) && pool.Count > 0)
            {
                obj = (TObject)pool.First;
                return true;
            }

            obj = null;
            return false;
        }

        public TObject Get<TId, TObject>(TId id) where TObject : class
        {
            var type = typeof(TObject);
            if (poolsByType.TryGetValue(type, out var pool))
            {
                var typedPool = (ObjectPool<TId, TObject>)pool;
                if (typedPool.TryGetValue(id, out var obj))
                {
                    return obj;
                }
            }

            throw new KeyNotFoundException($"Object of type {type.Name} with id {id} was not found");
        }

        public bool TryGet<TId, TObject>(TId id, out TObject obj) where TObject : class
        {
            var type = typeof(TObject);
            if (poolsByType.TryGetValue(type, out var pool))
            {
                var typedPool = (ObjectPool<TId, TObject>)pool;
                if (typedPool.TryGetValue(id, out obj))
                {
                    return true;
                }
            }

            obj = null;
            return false;
        }

        public void GetAll<TObject>(ICollection<TObject> buffer) where TObject : class
        {
            var type = typeof(TObject);
            if (objectsByType.TryGetValue(type, out var obj))
            {
                buffer.Add((TObject)obj);
            }

            if (poolsByType.TryGetValue(type, out var pool))
            {
                var poolObjects = pool.Objects;
                for (var index = 0; index < poolObjects.Count; index++)
                {
                    var poolObject = poolObjects[index];
                    buffer.Add((TObject)poolObject);
                }
            }
        }

        public void Add<TObject>(TObject obj) where TObject : class
        {
            if (obj == null)
            {
                Debug.LogError($"Object of type {typeof(TObject).Name} cannot be null");
                return;
            }

            var type = typeof(TObject);
            if (objectsByType.TryGetValue(type, out _))
            {
                Debug.LogWarning($"Could not add duplicate object {type.Name}");
                return;
            }

            objectsByType[type] = obj;

            if (bindingCountsByObject.TryGetValue(obj, out var count) == false)
            {
                allObjectIndices[obj] = allObjects.Count;
                allObjects.Add(obj);
                RegisterObject(obj);

                if (IsInitialized && obj is ILifecycleListener listener)
                {
                    listener.OnInitialized();
                }
            }

            bindingCountsByObject[obj] = count + 1;
        }

        public void Add<TId, TObject>(TId id, TObject obj) where TObject : class
        {
            if (obj == null)
            {
                Debug.LogError($"Object of type {typeof(TObject).Name} cannot be null");
                return;
            }

            var type = typeof(TObject);
            if (poolsByType.TryGetValue(type, out var pool) == false)
            {
                pool = new ObjectPool<TId, TObject>();
                poolsByType.Add(type, pool);
            }

            if (((ObjectPool<TId, TObject>)pool).TryAdd(id, obj))
            {
                if (bindingCountsByObject.TryGetValue(obj, out var count) == false)
                {
                    allObjectIndices[obj] = allObjects.Count;
                    allObjects.Add(obj);
                    RegisterObject(obj);

                    if (IsInitialized && obj is ILifecycleListener listener)
                    {
                        listener.OnInitialized();
                    }
                }

                bindingCountsByObject[obj] = count + 1;
            }
            else
            {
                Debug.LogWarning($"Could not add duplicate object {type.Name}, '{id}' already exists");
            }
        }

        public void Remove<TObject>() where TObject : class
        {
            var type = typeof(TObject);
            if (objectsByType.Remove(type, out var removed))
            {
                var count = bindingCountsByObject[removed] - 1;
                if (count <= 0)
                {
                    bindingCountsByObject.Remove(removed);
                    RemoveObject(removed);
                    UnregisterObject(removed);

                    if (removed is ILifecycleListener listener)
                    {
                        listener.OnDisposed();
                    }
                }
                else
                {
                    bindingCountsByObject[removed] = count;
                }
            }
        }

        public void Remove<TId, TObject>(TId id) where TObject : class
        {
            var objectType = typeof(TObject);
            if (poolsByType.TryGetValue(objectType, out var pool))
            {
                var typedPool = (ObjectPool<TId, TObject>)pool;

                if (typedPool.Remove(id, out var removed))
                {
                    var count = bindingCountsByObject[removed] - 1;
                    if (count <= 0)
                    {
                        bindingCountsByObject.Remove(removed);
                        RemoveObject(removed);
                        UnregisterObject(removed);

                        if (removed is ILifecycleListener listener)
                        {
                            listener.OnDisposed();
                        }
                    }
                    else
                    {
                        bindingCountsByObject[removed] = count;
                    }
                }

                if (typedPool.Count <= 0)
                {
                    poolsByType.Remove(objectType);
                }
            }
            else
            {
                PendingRemoveTypesBuffer.Clear();

                foreach (var (poolObjectType, otherPool) in poolsByType)
                {
                    if (objectType.IsAssignableFrom(poolObjectType))
                    {
                        if (otherPool.TryRemoveById(id, out var removed))
                        {
                            var count = bindingCountsByObject[removed] - 1;
                            if (count <= 0)
                            {
                                bindingCountsByObject.Remove(removed);
                                RemoveObject(removed);
                                UnregisterObject(removed);

                                if (removed is ILifecycleListener listener)
                                {
                                    listener.OnDisposed();
                                }
                            }
                            else
                            {
                                bindingCountsByObject[removed] = count;
                            }
                        }

                        if (otherPool.Count <= 0)
                        {
                            PendingRemoveTypesBuffer.Add(poolObjectType);
                        }
                    }
                }

                foreach (var removeType in PendingRemoveTypesBuffer)
                {
                    poolsByType.Remove(removeType);
                }
            }
        }

        private void RegisterObject(object obj)
        {
            for (var index = 0; index < allGroups.Count; index++)
            {
                var group = allGroups[index];
                group.AddObject(obj);
            }

            OnObjectRegistered?.Invoke(obj);
        }

        private void UnregisterObject(object obj)
        {
            for (var index = 0; index < allGroups.Count; index++)
            {
                var group = allGroups[index];
                group.RemoveObject(obj);
            }

            OnObjectUnregistered?.Invoke(obj);
        }

        private void RemoveObject(object obj)
        {
            var lastObject = allObjects[allObjects.Count - 1];
            var objectIndex = allObjectIndices[obj];

            allObjects[objectIndex] = lastObject;
            allObjectIndices[lastObject] = objectIndex;

            allObjects.RemoveAt(allObjects.Count - 1);
            allObjectIndices.Remove(obj);
        }

        private ObjectGroup<T> CreateGroup<T>() where T : class
        {
            var group = new ObjectGroup<T>();
            groupsByType[typeof(T)] = group;

            allGroups.Add(group);

            for (var index = 0; index < allObjects.Count; index++)
            {
                var obj = allObjects[index];
                group.AddObject(obj);
            }

            return group;
        }

        private ObjectGroup<T> CreateGroup<T>(Func<T, bool> predicate) where T : class
        {
            var group = new ObjectGroup<T>(predicate);
            var key = new PredicateKey(predicate, typeof(T));
            groupsByPredicate[key] = group;

            allGroups.Add(group);

            for (var index = 0; index < allObjects.Count; index++)
            {
                var obj = allObjects[index];
                group.AddObject(obj);
            }

            return group;
        }

        private readonly struct PredicateKey : IEquatable<PredicateKey>
        {
            private readonly int predicateHash;
            private readonly int typeHash;

            public PredicateKey(object predicate, Type type)
            {
                predicateHash = predicate.GetHashCode();
                typeHash = type.GetHashCode();
            }

            public bool Equals(PredicateKey other)
            {
                return predicateHash == other.predicateHash && typeHash == other.typeHash;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (predicateHash * 397) ^ typeHash;
                }
            }
        }
    }
}
