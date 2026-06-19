using InSun.GameCore.Messaging;

namespace InSun.GameCore
{
    public readonly struct ApplicationQuittingMessage : IMessage
    {
    }

    public readonly struct ObjectRegisteredMessage : IMessage
    {
        public object Object { get; }

        public ObjectRegisteredMessage(object @object)
        {
            Object = @object;
        }
    }

    public readonly struct ObjectUnregisteredMessage : IMessage
    {
        public object Object { get; }

        public ObjectUnregisteredMessage(object @object)
        {
            Object = @object;
        }
    }
}
