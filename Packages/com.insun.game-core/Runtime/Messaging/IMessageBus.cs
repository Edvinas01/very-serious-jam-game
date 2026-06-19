namespace InSun.GameCore.Messaging
{
    public interface IMessageBus
    {
        public void PublishMessage<T>(T message) where T : IMessage;

        public void AddListener<T>(OnMessage<T> callback) where T : IMessage;

        public void RemoveListener<T>(OnMessage<T> callback) where T : IMessage;
    }
}
