using Cysharp.Threading.Tasks;

namespace InSun.GameCore.Scenes
{
    public interface IScene
    {
        /// <summary>
        /// Path which can be used to load the scene.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// User-friendly name of the scene.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Unique id of this scene.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Load this scene!
        /// </summary>
        public UniTask LoadAsync(SceneLoadContext context);
    }
}
