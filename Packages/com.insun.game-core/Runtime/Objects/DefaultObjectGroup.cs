using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InSun.GameCore.Objects
{
    internal sealed class DefaultObjectGroup<T> : IObjectGroup<T> where T : class
    {
        public static DefaultObjectGroup<T> Instance { get; } = new();

        private DefaultObjectGroup()
        {
        }

        public int Count => 0;

        public T this[int index] => null;

        public event Action<T> OnObjectAdded
        {
            add { }
            remove { }
        }

        public event Action<T> OnObjectRemoved
        {
            add { }
            remove { }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
