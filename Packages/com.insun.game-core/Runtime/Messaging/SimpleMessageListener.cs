using System.Collections.Generic;

namespace InSun.GameCore.Messaging
{
    internal sealed class SimpleMessageListener<T> : SimpleMessageListener where T : IMessage
    {
        private readonly List<OnMessage<T>> listenerList = new();
        private readonly Dictionary<OnMessage<T>, int> listenerListIndices = new();

        public int ListenerCount => listenerList.Count;

        public void Publish(T message)
        {
            for (var index = listenerList.Count - 1; index >= 0; index--)
            {
                var listener = listenerList[index];
                listener.Invoke(message);
            }
        }

        public void AddListener(OnMessage<T> listener)
        {
            listenerListIndices[listener] = listenerList.Count;
            listenerList.Add(listener);
        }

        public void RemoveListener(OnMessage<T> listener)
        {
            if (listenerListIndices.TryGetValue(listener, out var listenerIndex) == false)
            {
                return;
            }

            var lastListener = listenerList[listenerList.Count - 1];
            listenerList[listenerIndex] = lastListener;
            listenerListIndices[lastListener] = listenerIndex;

            listenerList.RemoveAt(listenerList.Count - 1);
            listenerListIndices.Remove(listener);
        }
    }

    internal abstract class SimpleMessageListener
    {
    }
}
