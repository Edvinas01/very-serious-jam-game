using System;
using System.Collections.Generic;
using InSun.GameCore.Messaging;
using InSun.GameCore.Objects;

namespace InSun.GameCore
{
    public partial class Game
    {
        public static void PublishMessage<T>(T message) where T : IMessage
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.messageBus.PublishMessage(message);
        }

        public static void AddListener<T>(OnMessage<T> callback) where T : IMessage
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.messageBus.AddListener(callback);
        }

        public static void RemoveListener<T>(OnMessage<T> callback) where T : IMessage
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.messageBus.RemoveListener(callback);
        }

        public static TObject GetObject<TObject>() where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return null;
            }
#endif

            return instance.objectRegistry.Get<TObject>();
        }

        public static TObject GetObject<TId, TObject>(TId id) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return null;
            }
#endif

            return instance.objectRegistry.Get<TId, TObject>(id);
        }

        public static void GetObjects<TObject>(ICollection<TObject> objects) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            objects.Clear();
            instance.objectRegistry.GetAll(objects);
        }

        public static IObjectGroup<TObject> GetObjectGroup<TObject>() where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return DefaultObjectGroup<TObject>.Instance;
            }
#endif

            return instance.objectRegistry.GetGroup<TObject>();
        }

        public static IObjectGroup<TObject> GetObjectGroup<TObject>(Func<TObject, bool> predicate) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return DefaultObjectGroup<TObject>.Instance;
            }
#endif

            return instance.objectRegistry.GetGroup(predicate);
        }

        public static bool TryGetObject<TObject>(out TObject obj) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                obj = null;
                return false;
            }
#endif

            return instance.objectRegistry.TryGet(out obj);
        }

        public static bool TryGetObject<TId, TObject>(TId id, out TObject obj) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                obj = null;
                return false;
            }
#endif

            return instance.objectRegistry.TryGet(id, out obj);
        }

        public static void AddObject<TObject>(TObject obj) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.objectRegistry.Add(obj);
        }

        public static void AddObject<TId, TObject>(TId id, TObject obj) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.objectRegistry.Add(id, obj);
        }

        public static void RemoveObject<TObject>() where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.objectRegistry.Remove<TObject>();
        }

        public static void RemoveObject<TId, TObject>(TId id) where TObject : class
        {
#if UNITY_EDITOR
            if (IsInstanceReadyEditor() == false)
            {
                return;
            }
#endif

            instance.objectRegistry.Remove<TId, TObject>(id);
        }
    }
}
