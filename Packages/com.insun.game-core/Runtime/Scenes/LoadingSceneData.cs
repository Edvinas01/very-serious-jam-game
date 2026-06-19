using Cysharp.Threading.Tasks;
using InSun.GameCore.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InSun.GameCore.Scenes
{
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/Scenes/Loading Scene Data",
        fileName = MenuConstants.BaseAssetFileName + "Data_Scene"
    )]
    internal sealed class LoadingSceneData : SceneData
    {
        [Header("Resources")]
        [Tooltip("Run Resources.UnloadUnusedAssets() after loading the scene?")]
        [SerializeField]
        private bool isUnloadUnusedAssets = true;

        public override async UniTask LoadAsync(SceneLoadContext context)
        {
            // Actual loading
            var operation = SceneManager.LoadSceneAsync(Path);
            if (operation == null)
            {
                Debug.LogError($"Failed to load scene {Path}", this);
                return;
            }

            // Activation
            operation.allowSceneActivation = false;
            await UniTask.WaitUntil(operation, op => op.progress >= 0.9f);

            operation.allowSceneActivation = true;
            await UniTask.WaitUntil(operation, op => op.isDone);

            // Cleanup
            if (isUnloadUnusedAssets)
            {
                await Resources.UnloadUnusedAssets();
            }
        }
    }
}
