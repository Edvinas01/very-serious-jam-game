using System;
using System.Collections.Generic;
using UnityEditor;

namespace InSun.GameCore.Editor.SunnySettings
{
    internal sealed class SunnySettingsAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            var isAnyScriptableObjects = IsContainsScriptableObjects(importedAssets)
                || IsContainsScriptableObjects(deletedAssets)
                || IsContainsScriptableObjects(movedFromAssetPaths)
                || IsContainsScriptableObjects(importedAssets);

            if (isAnyScriptableObjects == false)
            {
                return;
            }

            if (EditorWindow.HasOpenInstances<SunnySettingsWindow>() == false)
            {
                return;
            }

            var window = EditorWindow.GetWindow<SunnySettingsWindow>(false,null,false);
            window.ReloadTree();
            window.Repaint();
        }

        private static bool IsContainsScriptableObjects(IReadOnlyList<string> assetPaths)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < assetPaths.Count; index++)
            {
                var path = assetPaths[index];
                if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
