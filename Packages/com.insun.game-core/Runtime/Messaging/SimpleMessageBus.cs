using System;
using System.Collections.Generic;

namespace InSun.GameCore.Messaging
{
    internal sealed class SimpleMessageBus : IMessageBus
    {
        private readonly Dictionary<Type, SimpleMessageListener> listenersByType = new();

        public void PublishMessage<T>(T message) where T : IMessage
        {
            var listenerType = typeof(T);
            if (listenersByType.TryGetValue(listenerType, out var listener))
            {
                var typedListener = (SimpleMessageListener<T>)listener;
                typedListener.Publish(message);
            }
        }

        public void AddListener<T>(OnMessage<T> callback) where T : IMessage
        {
            var listenerType = typeof(T);
            if (listenersByType.TryGetValue(listenerType, out var listener))
            {
                var typedListener = (SimpleMessageListener<T>)listener;
                typedListener.AddListener(callback);
            }
            else
            {
                var typedListener = new SimpleMessageListener<T>();
                typedListener.AddListener(callback);

                listenersByType[listenerType] = typedListener;
            }
        }

        public void RemoveListener<T>(OnMessage<T> callback) where T : IMessage
        {
            var listenerType = typeof(T);
            if (listenersByType.TryGetValue(listenerType, out var listener))
            {
                var typedListener = (SimpleMessageListener<T>)listener;
                typedListener.RemoveListener(callback);

                if (typedListener.ListenerCount <= 0)
                {
                    listenersByType.Remove(listenerType);
                }
            }
        }
    }
}
