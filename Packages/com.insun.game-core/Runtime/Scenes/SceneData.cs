using Cysharp.Threading.Tasks;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace InSun.GameCore.Scenes
{
    [SunnySettings(MenuPath = "Scenes")]
    public abstract class SceneData : ScriptableObject, IScene
    {
        [Header("Scenes")]
#if UNITY_EDITOR
        [SerializeField]
        private UnityEditor.SceneAsset sceneAsset;
#endif

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.LabelText("Path")]
#endif
        [SerializeField]
        private string scenePath;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.LabelText("GUID")]
#endif
        [SerializeField]
        private string sceneGuid;

        public string Path => scenePath;

        public virtual string Name => name;

        public string Id => sceneGuid;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (TryUpdateScenePathAndGuidEditor())
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif

        public abstract UniTask LoadAsync(SceneLoadContext context);

#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.TitleGroup("Debug", horizontalLine: false)]
        [Sirenix.OdinInspector.HorizontalGroup("Debug/Buttons")]
        [Sirenix.OdinInspector.DisableInPlayMode]
        [Sirenix.OdinInspector.Button("Open", Sirenix.OdinInspector.ButtonSizes.Medium)]
#endif
        private void OpenSceneEditor()
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(Path, UnityEditor.SceneManagement.OpenSceneMode.Single);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.TitleGroup("Debug", horizontalLine: false)]
        [Sirenix.OdinInspector.HorizontalGroup("Debug/Buttons")]
        [Sirenix.OdinInspector.DisableInEditorMode]
        [Sirenix.OdinInspector.Button("Load", Sirenix.OdinInspector.ButtonSizes.Medium)]
#endif
        private void LoadSceneEditor()
        {
            var sceneSystem = Game.GetObject<ISceneSystem>();
            if (sceneSystem.IsSceneLoading)
            {
                Debug.LogWarning("Already loading a scene", this);
                return;
            }

            var system = Game.GetObject<ISceneSystem>();
            system.LoadSceneAsync(new SceneLoadArgs(this)).Forget();
        }

        private bool TryUpdateScenePathAndGuidEditor()
        {
            if (Application.isPlaying || sceneAsset == false)
            {
                return false;
            }

            var scenePathPrev = scenePath;
            var scenePathNext = UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);

            var sceneGuidPrev = sceneGuid;
            var sceneGuidNext = UnityEditor.AssetDatabase.AssetPathToGUID(scenePathNext);
            if (string.Equals(scenePathPrev, scenePathNext) && string.Equals(sceneGuidPrev, sceneGuidNext))
            {
                return false;
            }

            scenePath = scenePathNext;
            sceneGuid = sceneGuidNext;

            return true;
        }

        private class SceneAssetProcessor : UnityEditor.AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (!IsAnyAssetRelevant(importedAssets) && !IsAnyAssetRelevant(movedAssets) && !IsAnyAssetRelevant(movedFromAssetPaths))
                {
                    return;
                }

                var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(SceneData)}");
                foreach (var guid in guids)
                {
                    var scenePath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                    var scene = UnityEditor.AssetDatabase.LoadAssetAtPath<SceneData>(scenePath);
                    if (scene == false)
                    {
                        continue;
                    }

                    if (scene.TryUpdateScenePathAndGuidEditor())
                    {
                        UnityEditor.EditorUtility.SetDirty(scene);
                        UnityEditor.AssetDatabase.SaveAssetIfDirty(scene);
                    }
                }
            }

            private static bool IsAnyAssetRelevant(string[] paths)
            {
                foreach (var path in paths)
                {
                    if (path.EndsWith(".unity") || path.EndsWith(".asset"))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
#endif
    }
}
