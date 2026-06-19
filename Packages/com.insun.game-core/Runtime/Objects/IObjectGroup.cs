using System;
using System.Collections.Generic;

namespace InSun.GameCore.Objects
{
    public interface IObjectGroup<out T> : IReadOnlyList<T> where T : class
    {
        public event Action<T> OnObjectAdded;

        public event Action<T> OnObjectRemoved;
    }
}
