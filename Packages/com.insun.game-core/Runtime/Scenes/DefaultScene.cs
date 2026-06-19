using Cysharp.Threading.Tasks;

namespace InSun.GameCore.Scenes
{
    public sealed class DefaultScene : IScene
    {
        public static DefaultScene Instance { get; } = new();

        public string Path => "";

        public string Name => "Default Scene";

        public string Id => "default-scene";

        public UniTask LoadAsync(SceneLoadContext context)
        {
            return UniTask.CompletedTask;
        }

        private DefaultScene()
        {
        }
    }
}
