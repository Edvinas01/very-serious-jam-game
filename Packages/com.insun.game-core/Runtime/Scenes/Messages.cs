using InSun.GameCore.Messaging;

namespace InSun.GameCore.Scenes
{
    public readonly struct SceneLoadStartedMessage : IMessage
    {
        public IScene SceneData { get; }

        public SceneLoadStartedMessage(IScene sceneData)
        {
            SceneData = sceneData;
        }
    }

    public readonly struct SceneActivationEnteredMessage : IMessage
    {
        public IScene SceneData { get; }

        public SceneActivationEnteredMessage(IScene sceneData)
        {
            SceneData = sceneData;
        }
    }

    public readonly struct SceneActivationExitedMessage : IMessage
    {
        public IScene SceneData { get; }

        public SceneActivationExitedMessage(IScene sceneData)
        {
            SceneData = sceneData;
        }
    }

    public readonly struct SceneLoadedMessage : IMessage
    {
        public IScene SceneData { get; }

        public SceneLoadedMessage(IScene sceneData)
        {
            SceneData = sceneData;
        }
    }
}
