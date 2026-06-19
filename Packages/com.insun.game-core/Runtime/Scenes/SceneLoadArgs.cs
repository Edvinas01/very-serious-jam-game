namespace InSun.GameCore.Scenes
{
    public sealed class SceneLoadArgs : ISceneLoadArgs
    {
        public IScene Scene { get; }

        public SceneLoadArgs(IScene scene)
        {
            Scene = scene;
        }
    }
}
