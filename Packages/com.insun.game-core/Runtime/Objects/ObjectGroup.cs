using System;
using System.Collections;
using System.Collections.Generic;

namespace InSun.GameCore.Objects
{
    internal sealed class ObjectGroup<T> : ObjectGroup, IObjectGroup<T> where T : class
    {
        private readonly List<T> objects = new();
        private readonly Dictionary<T, int> objectIndices = new();

        private readonly Func<T, bool> predicate;

        public int Count => objects.Count;

        public T this[int index] => objects[index];

        public event Action<T> OnObjectAdded;

        public event Action<T> OnObjectRemoved;

        public ObjectGroup()
        {
        }

        public ObjectGroup(Func<T, bool> predicate)
        {
            this.predicate = predicate;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        public override void AddObject(object obj)
        {
            if (obj is T typed == false)
            {
                return;
            }

            if (predicate != null && predicate(typed) == false)
            {
                return;
            }

            objectIndices[typed] = objects.Count;
            objects.Add(typed);
            OnObjectAdded?.Invoke(typed);
        }

        public override void RemoveObject(object obj)
        {
            if (obj is T typed)
            {
                RemoveObject(typed);
            }
        }

        private void RemoveObject(T obj)
        {
            if (objectIndices.TryGetValue(obj, out var objectIndex) == false)
            {
                return;
            }

            var lastObject = objects[objects.Count - 1];
            objects[objectIndex] = lastObject;
            objectIndices[lastObject] = objectIndex;

            objects.RemoveAt(objects.Count - 1);
            objectIndices.Remove(obj);
            OnObjectRemoved?.Invoke(obj);
        }
    }

    internal abstract class ObjectGroup
    {
        public abstract void AddObject(object obj);

        public abstract void RemoveObject(object obj);
    }
}
