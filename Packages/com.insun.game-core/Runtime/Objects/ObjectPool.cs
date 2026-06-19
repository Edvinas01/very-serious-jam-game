using System.Collections.Generic;

namespace InSun.GameCore.Objects
{
    internal sealed class ObjectPool<TId, TObject> : ObjectPool where TObject : class
    {
        private readonly Dictionary<TId, TObject> objects = new();
        private readonly List<object> objectList = new();
        private readonly Dictionary<object, int> objectListIndices = new();

        public override IReadOnlyList<object> Objects => objectList;

        public override object First
        {
            get
            {
                if (objectList.Count <= 0)
                {
                    return null;
                }

                return objectList[0];
            }
        }

        public override int Count => objects.Count;

        public bool TryGetValue(TId id, out TObject obj)
        {
            if (objects.TryGetValue(id, out var retrieved) && retrieved != null)
            {
                obj = retrieved;
                return true;
            }

            obj = default;
            return false;
        }

        public bool TryAdd(TId id, TObject obj)
        {
            if (objects.TryAdd(id, obj))
            {
                objectListIndices[obj] = objectList.Count;
                objectList.Add(obj);
                return true;
            }

            return false;
        }

        public bool Remove(TId id, out TObject obj)
        {
            if (objects.Remove(id, out obj))
            {
                RemoveObject(obj);
                return true;
            }

            return false;
        }

        public override bool TryRemoveById(object id, out object obj)
        {
            if (id is TId typedId && objects.Remove(typedId, out var removed))
            {
                RemoveObject(removed);
                obj = removed;
                return true;
            }

            obj = default;
            return false;
        }

        private void RemoveObject(object obj)
        {
            var lastObject = objectList[objectList.Count - 1];
            var objectIndex = objectListIndices[obj];

            objectList[objectIndex] = lastObject;
            objectListIndices[lastObject] = objectIndex;

            objectList.RemoveAt(objectList.Count - 1);
            objectListIndices.Remove(obj);
        }
    }

    internal abstract class ObjectPool
    {
        public abstract IReadOnlyList<object> Objects { get; }

        public abstract object First { get; }

        public abstract int Count { get; }

        public abstract bool TryRemoveById(object id, out object obj);
    }
}
