using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyWebGL
{
    internal sealed class SunnyWebGLAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths
        )
        {
            if (EditorWindow.HasOpenInstances<SunnyWebGLTemplateWindow>() == false)
            {
                return;
            }

            var window = EditorWindow.GetWindow<SunnyWebGLTemplateWindow>(false,null,false);
            window.RefreshState();
            window.Repaint();
        }
    }
}
