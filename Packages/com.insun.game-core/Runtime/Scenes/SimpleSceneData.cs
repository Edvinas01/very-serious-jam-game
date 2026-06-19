using Cysharp.Threading.Tasks;
using InSun.GameCore.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InSun.GameCore.Scenes
{
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/Scenes/Simple Scene Data",
        fileName = MenuConstants.BaseAssetFileName + "Data_Scene"
    )]
    public sealed class SimpleSceneData : SceneData, IScene
    {
        [Header("Loading")]
        [Tooltip(
            ""
            + "If specified, game will first load the specified scene before loading THIS scene - "
            + "useful for heavy scenes (prevents lag spike during load)"
        )]
        [SerializeField]
        private LoadingSceneData loadingScene;

        [Header("Identification")]
        [SerializeField]
        private string prettyName;

        [SerializeField]
        private Sprite icon;

        [Header("Resources")]
        [Tooltip("Run Resources.UnloadUnusedAssets() after loading the scene?")]
        [SerializeField]
        private bool isUnloadUnusedAssets;

        [Header("Fading")]
        [SerializeField]
        private bool isFadingEnabled = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isFadingEnabled))]
#endif
        [Min(0f)]
        [SerializeField]
        private float showFadeDuration = 0.3f;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isFadingEnabled))]
#endif
        [Min(0f)]
        [SerializeField]
        private float hideFadeDuration = 0.3f;

        [Header("Delay")]
        [SerializeField]
        private bool isPauseBeforeLoad = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isPauseBeforeLoad))]
#endif
        [Min(0f)]
        [SerializeField]
        private float loadPauseDuration = 0.3f;

        public override string Name => prettyName;

        public Sprite Icon => icon;

        public override async UniTask LoadAsync(SceneLoadContext context)
        {
            // Fade in
            if (isFadingEnabled)
            {
                UpdateLoadProgress(context.SceneSystem, 0f);
                await FadeInLoadingScreenAsync(context.SceneSystem);
            }

            // Enter hooks
            await context.SceneLoadEnteredHook.ExecuteAsync(context);

            if (loadingScene)
            {
                await loadingScene.LoadAsync(context);
            }

            // Pausing
            if (isPauseBeforeLoad && loadPauseDuration > 0f)
            {
                await UniTask.WaitForSeconds(loadPauseDuration, ignoreTimeScale: true);
            }

            // Actual loading
            var operation = SceneManager.LoadSceneAsync(Path);
            if (operation == null)
            {
                Debug.LogError($"Failed to load scene {Path}", this);
                return;
            }

            // Activation (NOTE, Start() is fired next frame after activation is done)
            operation.allowSceneActivation = false;
            await UniTask.WaitUntil(
                operation,
                op =>
                {
                    UpdateLoadProgress(context.SceneSystem, op.progress);
                    return op.progress >= 0.9f;
                }
            );

            await context.SceneActivationEnteredHook.ExecuteAsync(context);
            operation.allowSceneActivation = true;
            await UniTask.WaitUntil(
                operation,
                op =>
                {
                    UpdateLoadProgress(context.SceneSystem, 1f);
                    return op.isDone;
                }
            );

            await context.SceneActivationExitedHook.ExecuteAsync(context);

            // Cleanup
            if (isUnloadUnusedAssets)
            {
                await Resources.UnloadUnusedAssets();
            }

            // Exit hooks
            await context.SceneLoadExitedHook.ExecuteAsync(context);

            // Fade out
            if (isFadingEnabled)
            {
                await FadeOutLoadingScreenAsync(context.SceneSystem);
            }
        }

        private async UniTask FadeInLoadingScreenAsync(ISceneSystem sceneSystem)
        {
            if (sceneSystem is not SimpleSceneSystem simpleSceneSystem)
            {
                return;
            }

            var loadingViewController = simpleSceneSystem.LoadingSceneViewController;
            loadingViewController.Initialize(showFadeDuration, hideFadeDuration);
            await loadingViewController.ShowViewAsync();
        }

        private async UniTask FadeOutLoadingScreenAsync(ISceneSystem sceneSystem)
        {
            if (sceneSystem is not SimpleSceneSystem simpleSceneSystem)
            {
                return;
            }

            var loadingViewController = simpleSceneSystem.LoadingSceneViewController;
            await loadingViewController.HideViewAsync();
        }

        private void UpdateLoadProgress(ISceneSystem sceneSystem, float progress)
        {
            if (sceneSystem is not SimpleSceneSystem simpleSceneSystem)
            {
                return;
            }

            var loadingViewController = simpleSceneSystem.LoadingSceneViewController;
            loadingViewController.LoadProgress = progress;
        }
    }
}
