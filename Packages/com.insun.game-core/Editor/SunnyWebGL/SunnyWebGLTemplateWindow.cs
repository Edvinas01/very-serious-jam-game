using System.IO;
using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyWebGL
{
    internal sealed class SunnyWebGLTemplateWindow : EditorWindow
    {
        private const string TemplatePathDestination = "Assets/WebGLTemplates/SunnyWebGL";
        private const string TemplatePathSource = "Packages/com.insun.game-core/Editor/SunnyWebGL/Template~";

        private const string FileFavicon = "TemplateData/favicon.ico";
        private const string FileLoadingIcon = "TemplateData/icon-loading.png";

        private bool isTemplateImported;

        [MenuItem(
            MenuConstants.BaseWindowMenuName + "/Sunny WebGL",
            priority = MenuConstants.BaseWindowMenuPriority
        )]
        private static void OpenWindow()
        {
            var window = GetWindow<SunnyWebGLTemplateWindow>();
            window.titleContent = new GUIContent("Sunny WebGL Template", SunnyWebGLIcons.WebGLIcon);
            window.minSize = new Vector2(350, 250);
            window.Show();
        }

        private void OnEnable()
        {
            RefreshState();
        }

        public void RefreshState()
        {
            isTemplateImported = Directory.Exists(TemplatePathDestination);
        }

        private void OnGUI()
        {
            DrawImportSection();

            if (isTemplateImported)
            {
                DrawAssetReplacement(
                    label: "Favicon",
                    description: "Replace favicon.ico shown in the browser tab",
                    destinationFileName: FileFavicon
                );

                DrawAssetReplacement(
                    label: "Loading Icon",
                    description: "Replace icon-loading.png shown on the loading screen",
                    destinationFileName: FileLoadingIcon
                );
            }
        }

        private void DrawImportSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var statusLabel = isTemplateImported
                ? "Template imported at: " + TemplatePathDestination
                : "Template not imported";

            var statusStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                normal =
                {
                    textColor = isTemplateImported ? Color.limeGreen : Color.crimson
                },
            };

            EditorGUILayout.LabelField(statusLabel, statusStyle);
            EditorGUILayout.Space(4);

            var buttonLabel = isTemplateImported ? "Re-import Template" : "Import Template";
            if (GUILayout.Button(buttonLabel, GUILayout.Height(32)))
            {
                ImportTemplate();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawAssetReplacement(string label, string description, string destinationFileName)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(description, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.Space(4);

            if (GUILayout.Button($"Replace {label}", GUILayout.Height(32)))
            {
                ReplaceAsset(destinationFileName);
            }

            EditorGUILayout.EndVertical();
        }

        private void ImportTemplate()
        {
            var targetDirectory = "Assets/WebGLTemplates";
            if (Directory.Exists(targetDirectory) == false)
            {
                Directory.CreateDirectory(targetDirectory);
                AssetDatabase.ImportAsset(targetDirectory);
            }

            if (Directory.Exists(TemplatePathDestination))
            {
                var isConfirmed = EditorUtility.DisplayDialog(
                    title: "Re-import Template",
                    message: $"This will overwrite the existing template at: {TemplatePathDestination}",
                    ok: "Overwrite",
                    cancel: "Cancel"
                );

                if (isConfirmed == false)
                {
                    return;
                }

                FileUtil.DeleteFileOrDirectory(TemplatePathDestination);
                FileUtil.DeleteFileOrDirectory(TemplatePathDestination + ".meta");
            }

            FileUtil.CopyFileOrDirectory(TemplatePathSource, TemplatePathDestination);

            AssetDatabase.Refresh();
            RefreshState();

            EditorUtility.DisplayDialog(
                title: "Import Successful",
                message: $"WebGL template imported to: {TemplatePathDestination}",
                ok: "Ok"
            );
        }

        private void ReplaceAsset(string destinationFileName)
        {
            var destinationPath = Path.Combine(TemplatePathDestination, destinationFileName).Replace('\\', '/');
            var sourcePath = EditorUtility.OpenFilePanel(
                $"Select replacement for {destinationFileName}",
                Application.dataPath,
                GetExtension(destinationFileName)
            );

            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                return;
            }

            var expectedExt = ("." + GetExtension(destinationFileName)).ToLowerInvariant();
            var chosenExt = Path.GetExtension(sourcePath).ToLowerInvariant();

            if (expectedExt != chosenExt)
            {
                EditorUtility.DisplayDialog(
                    title: "Invalid File",
                    message: $"Expected a *{expectedExt} file but got *{chosenExt}",
                    ok: "Ok"
                );
                return;
            }

            File.Copy(sourcePath, destinationPath, overwrite: true);
            AssetDatabase.ImportAsset(destinationPath, ImportAssetOptions.ForceUpdate);

            Repaint();

            Debug.Log($"Replaced \"{destinationPath}\" with \"{sourcePath}\"");
        }

        private static string GetExtension(string filename)
        {
            return Path.GetExtension(filename).TrimStart('.').ToLower();
        }
    }
}
