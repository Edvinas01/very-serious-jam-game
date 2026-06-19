using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using InSun.GameCore.Objects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace InSun.GameCore.Scenes
{
    public sealed class SimpleSceneSystem : MonoBehaviour, ISceneSystem, ILifecycleListener
    {
        [Header("Scenes")]
        [SerializeField]
        private List<SceneData> scenes = new();

        [FormerlySerializedAs("fadeViewController")]
        [Header("UI")]
        [SerializeField]
        private LoadingSceneViewController loadingSceneViewController;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onSceneLoadStarted;

        [SerializeField]
        private UnityEvent onSceneLoaded;

        public bool IsSceneLoading => loadingScene != null;

        public bool IsSceneLoaded => loadedScene != null;

        public LoadingSceneViewController LoadingSceneViewController => loadingSceneViewController;

        public ISceneLoadHook SceneLoadEnteredHook { get; set; }

        public ISceneLoadHook SceneLoadExitedHook { get; set; }

        public ISceneLoadHook SceneActivationEnteredHook { get; set; }

        public ISceneLoadHook SceneActivationExitedHook { get; set; }

        private IScene loadingScene;
        private IScene loadedScene;

        public void OnInitialized()
        {
        }

        public void OnDisposed()
        {
        }

        public bool IsLoaded(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            if (loadedScene == null)
            {
                return false;
            }

            return string.Equals(loadedScene.Id, id);
        }

        public bool IsLoaded(IScene scene)
        {
            if (loadedScene == null)
            {
                return false;
            }

            return loadedScene == scene;
        }

        public bool TryGetLoadingScene(out IScene scene)
        {
            if (loadingScene == null)
            {
                scene = null;
                return false;
            }

            scene = loadingScene;
            return true;
        }

        public bool TryGetLoadedScene(out IScene scene)
        {
            if (loadedScene != null)
            {
                scene = loadedScene;
                return true;
            }

            var sceneDataList = Resources.FindObjectsOfTypeAll<SceneData>();
            var activeScene = SceneManager.GetActiveScene();

            foreach (var sceneData in sceneDataList)
            {
                if (activeScene.path == sceneData.Path)
                {
                    loadedScene = sceneData;
                    scene = loadedScene;
                    return true;
                }
            }

            scene = null;
            return false;
        }

        public bool TryGetScene(string id, out IScene scene)
        {
            foreach (var sceneData in scenes)
            {
                if (sceneData && sceneData.Id == id)
                {
                    scene = sceneData;
                    return true;
                }
            }

            scene = null;
            return false;
        }

        public void LoadScene(ISceneLoadArgs args)
        {
            LoadSceneInternalAsync(args).Forget();
        }

        public UniTask LoadSceneAsync(ISceneLoadArgs args)
        {
            return LoadSceneInternalAsync(args);
        }

        private async UniTask LoadSceneInternalAsync(ISceneLoadArgs args)
        {
            var scene = args.Scene;
            if (loadingScene != null)
            {
                Debug.LogWarning(
                    ""
                    + $"Cannot load scene \"{scene.Path}\", "
                    + $"scene \"{loadingScene.Path}\" is already loading",
                    this
                );

                return;
            }

            loadingScene = scene;

            try
            {
                Game.PublishMessage(new SceneLoadStartedMessage(scene));
                onSceneLoadStarted.Invoke();

                // TODO: I don't really like the hook system, maybe we can do it in a better way? Works for now tho...
                var context = new SceneLoadContext(
                    sceneSystem: this,
                    sceneLoadArgs: args,
                    sceneLoadEnteredHook: SceneLoadEnteredHook ?? DefaultSceneLoadHook.Instance,
                    sceneLoadExitedHook: new WrapperLoadExitedHook(
                        wrappedHook: SceneLoadExitedHook ?? DefaultSceneLoadHook.Instance,
                        sceneSystem: this
                    ),
                    sceneActivationEnteredHook: new WrapperActivationEnteredHook(
                        SceneActivationEnteredHook ?? DefaultSceneLoadHook.Instance,
                        sceneSystem: this
                    ),
                    sceneActivationExitedHook: new WrapperActivationExitedHook(
                        wrappedHook: SceneActivationExitedHook ?? DefaultSceneLoadHook.Instance
                    )
                );

                Debug.Log($"Loading scene {scene.Name}", this);
                await scene.LoadAsync(context);
            }
            finally
            {
                loadingScene = null;
            }
        }

        private sealed class WrapperActivationEnteredHook : ISceneLoadHook
        {
            private readonly ISceneLoadHook wrappedHook;
            private readonly SimpleSceneSystem sceneSystem;

            public WrapperActivationEnteredHook(ISceneLoadHook wrappedHook, SimpleSceneSystem sceneSystem)
            {
                this.wrappedHook = wrappedHook;
                this.sceneSystem = sceneSystem;
            }

            public async UniTask ExecuteAsync(SceneLoadContext context)
            {
                sceneSystem.loadedScene = context.SceneLoadArgs.Scene;
                sceneSystem.loadingScene = null;

                await wrappedHook.ExecuteAsync(context);
                Game.PublishMessage(new SceneActivationEnteredMessage(context.SceneLoadArgs.Scene));
            }
        }

        private sealed class WrapperActivationExitedHook : ISceneLoadHook
        {
            private readonly ISceneLoadHook wrappedHook;

            public WrapperActivationExitedHook(ISceneLoadHook wrappedHook)
            {
                this.wrappedHook = wrappedHook;
            }

            public async UniTask ExecuteAsync(SceneLoadContext context)
            {
                await wrappedHook.ExecuteAsync(context);
                Game.PublishMessage(new SceneActivationExitedMessage(context.SceneLoadArgs.Scene));
            }
        }

        private sealed class WrapperLoadExitedHook : ISceneLoadHook
        {
            private readonly ISceneLoadHook wrappedHook;
            private readonly SimpleSceneSystem sceneSystem;

            public WrapperLoadExitedHook(ISceneLoadHook wrappedHook, SimpleSceneSystem sceneSystem)
            {
                this.wrappedHook = wrappedHook;
                this.sceneSystem = sceneSystem;
            }

            public async UniTask ExecuteAsync(SceneLoadContext context)
            {
                sceneSystem.loadedScene = context.SceneLoadArgs.Scene;
                sceneSystem.loadingScene = null;

                await wrappedHook.ExecuteAsync(context);

                Debug.Log($"Loaded scene {context.SceneLoadArgs.Scene.Name}", sceneSystem);
                Game.PublishMessage(new SceneLoadedMessage(context.SceneLoadArgs.Scene));
                sceneSystem.onSceneLoaded.Invoke();
            }
        }
    }
}
