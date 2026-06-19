using Cysharp.Threading.Tasks;

namespace InSun.GameCore.Scenes
{
    public interface ISceneLoadHook
    {
        public UniTask ExecuteAsync(SceneLoadContext context);
    }
}
