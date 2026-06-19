using Cysharp.Threading.Tasks;

namespace InSun.GameCore.Scenes
{
    internal sealed class DefaultSceneLoadHook : ISceneLoadHook
    {
        public static DefaultSceneLoadHook Instance { get; } = new();

        public UniTask ExecuteAsync(SceneLoadContext context)
        {
            return UniTask.CompletedTask;
        }
    }
}
