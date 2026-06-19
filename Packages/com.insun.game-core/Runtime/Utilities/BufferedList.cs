using System;
using System.Collections;
using System.Collections.Generic;

namespace InSun.GameCore.Utilities
{
    public sealed class BufferedList<T> : IList<T>, IReadOnlyList<T>
    {
        public static readonly BufferedList<T> Empty = new(maxSize: 0);

        private readonly T[] items;

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public BufferedList(int maxSize = 2048)
        {
            items = new T[maxSize];
        }

        public void Clear()
        {
            Count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(items, item, 0, Count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(
                sourceArray: items,
                sourceIndex: 0,
                destinationArray: array,
                destinationIndex: arrayIndex,
                length: Count
            );
        }

        public bool TryAdd(T item)
        {
#if UNITY_EDITOR
            if (Count >= items.Length)
            {
                UnityEngine.Debug.LogWarning($"{nameof(BufferedList<T>)} exceeded buffer size of {items.Length} for type {typeof(T).Name}");
                return false;
            }
#endif

            items[Count++] = item;
            return true;
        }

        public void Add(T item)
        {
            if (TryAdd(item) == false)
            {
                throw new InvalidOperationException($"{nameof(BufferedList<T>)} is at capacity ({items.Length})");
            }
        }

        public void Insert(int index, T item)
        {
            if (Count >= items.Length)
            {
                throw new InvalidOperationException($"{nameof(BufferedList<T>)} is at capacity ({items.Length})");
            }

            Array.Copy(
                sourceArray: items,
                sourceIndex: index,
                destinationArray: items,
                destinationIndex: index + 1,
                length: Count - index
            );

            items[index] = item;
            Count++;
        }

        public void RemoveAt(int index)
        {
            Count--;
            Array.Copy(
                sourceArray: items,
                sourceIndex: index + 1,
                destinationArray: items,
                destinationIndex: index,
                length: Count - index
            );

            items[Count] = default;
        }

        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void Sort(IComparer<T> comparer)
        {
            Array.Sort(items, 0, Count, comparer);
        }

        public void TruncateTo(int count)
        {
            if (count >= Count)
            {
                return;
            }

            Count = count;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            private readonly BufferedList<T> buffer;
            private int index;

            internal Enumerator(BufferedList<T> buffer)
            {
                this.buffer = buffer;
                index = -1;
            }

            public T Current => buffer.items[index];

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                return ++index < buffer.Count;
            }

            public void Reset()
            {
                index = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}
