using System;
using System.Collections.Generic;

namespace InSun.GameCore.Objects
{
    public interface IObjectRegistry
    {
        public bool IsInitialized { set; }

        public event Action<object> OnObjectRegistered;

        public event Action<object> OnObjectUnregistered;

        public void Initialize();

        public void Dispose();

        public void FixedUpdate();

        public void Update();

        public void LateUpdate();

        public IObjectGroup<TObject> GetGroup<TObject>() where TObject : class;

        public IObjectGroup<TObject> GetGroup<TObject>(Func<TObject, bool> predicate) where TObject : class;

        public TObject Get<TObject>() where TObject : class;

        public bool TryGet<TObject>(out TObject obj) where TObject : class;

        public TObject Get<TId, TObject>(TId id) where TObject : class;

        public bool TryGet<TId, TObject>(TId id, out TObject obj) where TObject : class;

        public void GetAll<TObject>(ICollection<TObject> buffer) where TObject : class;

        public void Add<TObject>(TObject obj) where TObject : class;

        public void Add<TId, TObject>(TId id, TObject obj) where TObject : class;

        public void Remove<TObject>() where TObject : class;

        public void Remove<TId, TObject>(TId id) where TObject : class;
    }
}
