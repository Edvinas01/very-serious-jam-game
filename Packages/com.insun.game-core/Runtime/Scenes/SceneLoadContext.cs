namespace InSun.GameCore.Scenes
{
    public readonly struct SceneLoadContext
    {
        public ISceneSystem SceneSystem { get; }

        public ISceneLoadArgs SceneLoadArgs { get; }

        public ISceneLoadHook SceneLoadEnteredHook { get; }

        public ISceneLoadHook SceneLoadExitedHook { get; }

        public ISceneLoadHook SceneActivationEnteredHook { get; }

        public ISceneLoadHook SceneActivationExitedHook { get; }

        public SceneLoadContext(
            ISceneSystem sceneSystem,
            ISceneLoadArgs sceneLoadArgs,
            ISceneLoadHook sceneLoadEnteredHook,
            ISceneLoadHook sceneLoadExitedHook,
            ISceneLoadHook sceneActivationEnteredHook,
            ISceneLoadHook sceneActivationExitedHook
        )
        {
            SceneSystem = sceneSystem;
            SceneLoadArgs = sceneLoadArgs;
            SceneLoadEnteredHook = sceneLoadEnteredHook;
            SceneLoadExitedHook = sceneLoadExitedHook;
            SceneActivationEnteredHook = sceneActivationEnteredHook;
            SceneActivationExitedHook = sceneActivationExitedHook;
        }
    }
}
