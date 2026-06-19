using Cysharp.Threading.Tasks;

namespace InSun.GameCore.Scenes
{
    public interface ISceneSystem
    {
        public bool IsSceneLoading { get; }

        public bool IsSceneLoaded { get; }

        public bool IsLoaded(string id);

        public bool IsLoaded(IScene scene);

        public bool TryGetLoadingScene(out IScene scene);

        public bool TryGetLoadedScene(out IScene scene);

        public bool TryGetScene(string id, out IScene scene);

        public void LoadScene(ISceneLoadArgs args);

        public UniTask LoadSceneAsync(ISceneLoadArgs args);
    }
}
