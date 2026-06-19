using System;
using System.IO;
using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace InSun.GameCore.Editor.SunnyScreenshots
{
    internal sealed class SunnyScreenshotsWindow : EditorWindow
    {
        private enum ScreenshotType
        {
            Regular = 0,
            Transparent = 1,
            Cubemap = 2,
        }

        private static readonly string OutputPathKey = typeof(SunnyScreenshotsWindow).FullName + ".OutputPath";
        private const string DefaultOutputPath = "Assets/Personal/Screenshots";

        private static readonly string[] CubemapSizeLabels = { "512", "1024", "2048", "4096" };
        private static readonly int[] CubemapSizes = { 512, 1024, 2048, 4096 };

        [SerializeField]
        private ScreenshotType screenshotType;

        [SerializeField]
        private bool isUseSceneViewDimensions;

        [SerializeField]
        private int cubemapSize = 1024;

        [SerializeField]
        private int screenshotWidth = 1920;

        [SerializeField]
        private int screenshotHeight = 1080;

        private string outputPath;

        [MenuItem(
            MenuConstants.BaseWindowMenuName + "/Sunny Screenshots",
            priority = MenuConstants.BaseWindowMenuPriority
        )]
        public static void OpenWindow()
        {
            var window = GetWindow<SunnyScreenshotsWindow>();
            window.titleContent = new GUIContent("Sunny Screenshots", SunnyScreenshotsIcons.CameraIcon);
            window.minSize = new Vector2(200, 100);
            window.Show();
        }

        private void OnEnable()
        {
            outputPath = EditorPrefs.GetString(OutputPathKey, DefaultOutputPath);
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            DrawFeaturesSection();
            EditorGUILayout.Space(4f);
            DrawSizeSection();
            EditorGUILayout.Space(4f);
            DrawOutputSection();
            EditorGUILayout.Space(4f);
            DrawActionsSection();
        }

        private void DrawFeaturesSection()
        {
            EditorGUILayout.LabelField("Features", EditorStyles.boldLabel);

            screenshotType = (ScreenshotType)EditorGUILayout.EnumPopup("Screenshot Type", screenshotType);
        }

        private void DrawSizeSection()
        {
            EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);

            if (screenshotType == ScreenshotType.Cubemap)
            {
                cubemapSize = EditorGUILayout.IntPopup("Cubemap Size", cubemapSize, CubemapSizeLabels, CubemapSizes);
            }
            else
            {
                isUseSceneViewDimensions = EditorGUILayout.Toggle("Use Scene View Dimensions", isUseSceneViewDimensions);

                if (!isUseSceneViewDimensions)
                {
                    screenshotWidth = Mathf.Max(0, EditorGUILayout.IntField("Width", screenshotWidth));
                    screenshotHeight = Mathf.Max(0, EditorGUILayout.IntField("Height", screenshotHeight));
                }
            }
        }

        private void DrawOutputSection()
        {
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            outputPath = EditorGUILayout.TextField("Path", outputPath);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(OutputPathKey, outputPath);
            }

            if (GUILayout.Button("Browse", GUILayout.Width(55f)))
            {
                TryBrowseOutputPath();
            }

            if (GUILayout.Button("Reset", GUILayout.Width(45f)))
            {
                ResetOutputPath();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawActionsSection()
        {
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Take Screenshot", GUILayout.Height(30f)))
            {
                TakeScreenshot();
            }
        }

        private void TryBrowseOutputPath()
        {
            var absoluteDefault = Path.GetFullPath(Path.Combine(Application.dataPath, "..", outputPath));
            var selected = EditorUtility.OpenFolderPanel(
                title: "Select Screenshot Output Folder",
                folder: Directory.Exists(absoluteDefault) ? absoluteDefault : Application.dataPath,
                defaultName: string.Empty
            );

            if (string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            var projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            if (selected.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
            {
                selected = selected.Substring(projectRoot.Length).TrimStart('/', '\\');
            }

            outputPath = selected;
            EditorPrefs.SetString(OutputPathKey, outputPath);
        }

        private void ResetOutputPath()
        {
            outputPath = DefaultOutputPath;
            EditorPrefs.SetString(OutputPathKey, outputPath);
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!HasOpenInstances<SunnyScreenshotsWindow>())
            {
                return;
            }

            var camera = sceneView.camera;
            if (camera == false)
            {
                return;
            }

            Handles.BeginGUI();

            var sceneDimensions = sceneView.position;
            var sceneWidth = sceneDimensions.width;
            var sceneHeight = sceneDimensions.height;

            var lineColor = Color.white;
            lineColor.a = 0.5f;

            DrawLine(
                new Vector2(sceneWidth / 2f, 0f),
                new Vector2(sceneWidth / 2f, sceneHeight),
                lineColor
            );

            DrawLine(
                new Vector2(0f, sceneHeight / 2f),
                new Vector2(sceneWidth, sceneHeight / 2f),
                lineColor
            );

            Handles.EndGUI();

            sceneView.Repaint();

            return;

            void DrawLine(Vector2 start, Vector2 end, Color color)
            {
                Handles.color = color;
                Handles.DrawAAPolyLine(2f, start, end);
                Handles.color = Color.white;
            }
        }

        private void TakeScreenshot()
        {
            var sceneView = SceneView.lastActiveSceneView;
            if (sceneView == false)
            {
                Debug.LogWarning("Open the scene window before taking a screenshot", this);
                return;
            }

            switch (screenshotType)
            {
                case ScreenshotType.Regular:
                {
                    var screenshot = TakeRegularScreenshot(sceneView);
                    SaveScreenshot(screenshot);
                    break;
                }
                case ScreenshotType.Transparent:
                {
                    var screenshot = TakeTransparentScreenshot(sceneView);
                    SaveScreenshot(screenshot);
                    break;
                }
                case ScreenshotType.Cubemap:
                {
                    var screenshot = TakeCubemapScreenshot(sceneView);
                    SaveScreenshot(screenshot);
                    break;
                }
                default:
                {
                    Debug.LogWarning($"Unsupported {nameof(screenshotType)} {screenshotType}", this);
                    break;
                }
            }
        }

        private Texture2D TakeRegularScreenshot(SceneView sceneView)
        {
            var dimensions = GetScreenshotDimensions(sceneView);
            var camera = sceneView.camera;

            var screenshotResultTexture = new Texture2D(
                width: dimensions.x,
                height: dimensions.y,
                textureFormat: TextureFormat.ARGB32,
                mipChain: false,
                linear: true
            );

            var renderTexture = RenderTexture.GetTemporary(
                width: dimensions.x,
                height: dimensions.y,
                depthBuffer: 24,
                RenderTextureFormat.ARGB32
            );

            var originalActiveTexture = RenderTexture.active;
            var originalCameraTargetTexture = camera.targetTexture;

            try
            {
                RenderTexture.active = renderTexture;
                camera.targetTexture = renderTexture;

                camera.Render();

                var grabArea = new Rect(0, 0, dimensions.x, dimensions.y);
                screenshotResultTexture.ReadPixels(grabArea, 0, 0);
                screenshotResultTexture.Apply();

                return screenshotResultTexture;
            }
            finally
            {
                camera.targetTexture = originalCameraTargetTexture;
                RenderTexture.active = originalActiveTexture;

                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        // Based on:
        // https://gist.github.com/ogxd/48527a80039b778792b3f4ece8db013c
        private Texture2D TakeTransparentScreenshot(SceneView sceneView)
        {
            var dimensions = GetScreenshotDimensions(sceneView);
            var camera = sceneView.camera;

            var screenshotResultTexture = new Texture2D(
                width: dimensions.x,
                height: dimensions.y,
                textureFormat: TextureFormat.ARGB32,
                mipChain: false,
                linear: true
            );

            var screenshotWhiteBackgroundTexture = new Texture2D(
                width: dimensions.x,
                height: dimensions.y,
                textureFormat: TextureFormat.ARGB32,
                mipChain: false
            );

            var screenshotBlackBackgroundTexture = new Texture2D(
                width: dimensions.x,
                height: dimensions.y,
                textureFormat: TextureFormat.ARGB32,
                mipChain: false
            );

            var renderTexture = RenderTexture.GetTemporary(
                width: dimensions.x,
                height: dimensions.y,
                depthBuffer: 24,
                RenderTextureFormat.ARGB32
            );

            var originalActiveTexture = RenderTexture.active;
            var originalCameraTargetTexture = camera.targetTexture;
            var originalCameraBackground = camera.backgroundColor;
            var originalCameraClearFlags = camera.clearFlags;

            try
            {
                RenderTexture.active = renderTexture;
                camera.targetTexture = renderTexture;
                camera.backgroundColor = Color.clear;
                camera.clearFlags = CameraClearFlags.SolidColor;

                var grabArea = new Rect(0, 0, dimensions.x, dimensions.y);

                // Black
                {
                    camera.backgroundColor = Color.black;
                    camera.Render();

                    screenshotBlackBackgroundTexture.ReadPixels(grabArea, 0, 0);
                    screenshotBlackBackgroundTexture.Apply();
                }

                {
                    // White
                    camera.backgroundColor = Color.white;
                    camera.Render();

                    screenshotWhiteBackgroundTexture.ReadPixels(grabArea, 0, 0);
                    screenshotWhiteBackgroundTexture.Apply();
                }

                var whitePixels = screenshotWhiteBackgroundTexture.GetPixels();
                var blackPixels = screenshotBlackBackgroundTexture.GetPixels();

                // Create Alpha from the difference between black and white camera renders
                for (var y = 0; y < dimensions.y; y++)
                {
                    for (var x = 0; x < dimensions.x; x++)
                    {
                        var alpha = whitePixels[x + y * dimensions.x].r - blackPixels[x + y * dimensions.x].r;
                        alpha = 1.0f - alpha;

                        Color color;
                        if (alpha == 0)
                        {
                            color = Color.clear;
                        }
                        else
                        {
                            color = blackPixels[x + y * dimensions.x] / alpha;
                        }

                        color.a = alpha;
                        blackPixels[x + y * dimensions.x] = color;
                    }
                }

                screenshotResultTexture.SetPixels(blackPixels);

                return screenshotResultTexture;
            }
            finally
            {
                RenderTexture.active = originalActiveTexture;
                camera.targetTexture = originalCameraTargetTexture;
                camera.backgroundColor = originalCameraBackground;
                camera.clearFlags = originalCameraClearFlags;

                DestroyImmediate(screenshotWhiteBackgroundTexture);
                DestroyImmediate(screenshotBlackBackgroundTexture);

                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        private Texture2D TakeCubemapScreenshot(SceneView sceneView)
        {
            var dimensions = GetScreenshotDimensions(sceneView);
            var camera = sceneView.camera;

            var screenshotResultTexture = new Texture2D(
                width: dimensions.x * 2,
                height: dimensions.y,
                textureFormat: TextureFormat.RGB24,
                mipChain: false,
                linear: true
            );

            var screenshotCubeTexture = new RenderTexture(
                width: dimensions.x * 2,
                height: dimensions.y,
                depth: 24
            );

            var renderTexture = RenderTexture.GetTemporary(width: dimensions.x, height: dimensions.y, depthBuffer: 24);
            renderTexture.dimension = TextureDimension.Cube;

            var originalActiveTexture = RenderTexture.active;
            var originalCameraTargetTexture = camera.targetTexture;

            try
            {
                camera.RenderToCubemap(
                    cubemap: renderTexture,
                    faceMask: 63, // 63 = render all faces
                    stereoEye: Camera.MonoOrStereoscopicEye.Mono
                );

                renderTexture.ConvertToEquirect(screenshotCubeTexture);

                var grabArea = new Rect(0, 0, dimensions.x * 2, dimensions.y);
                screenshotResultTexture.ReadPixels(grabArea, 0, 0);
                screenshotResultTexture.Apply();

                return screenshotResultTexture;
            }
            finally
            {
                RenderTexture.active = originalActiveTexture;
                camera.targetTexture = originalCameraTargetTexture;

                DestroyImmediate(screenshotCubeTexture);

                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        private Vector2Int GetScreenshotDimensions(SceneView sceneView)
        {
            var sceneViewRect = sceneView.position;

            if (screenshotType == ScreenshotType.Cubemap)
            {
                return new Vector2Int(cubemapSize, cubemapSize);
            }

            return isUseSceneViewDimensions
                ? new Vector2Int((int)sceneViewRect.width, (int)sceneViewRect.height)
                : new Vector2Int(screenshotWidth, screenshotHeight);
        }

        private void SaveScreenshot(Texture2D screenshot)
        {
            var absoluteOutputPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", outputPath));
            if (Directory.Exists(absoluteOutputPath) == false)
            {
                Directory.CreateDirectory(absoluteOutputPath);
            }

            var outputFileName = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene.IsValid())
            {
                outputFileName = $"{activeScene.name}_{outputFileName}";
            }

            var absoluteFilePath = Path.Combine(absoluteOutputPath, outputFileName);

            File.WriteAllBytes(absoluteFilePath, screenshot.EncodeToPNG());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            var assetPath = $"{outputPath}/{outputFileName}";
            if (screenshotType == ScreenshotType.Cubemap)
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.textureShape = TextureImporterShape.TextureCube;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.SaveAndReimport();
            }
            else
            {
                var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.textureShape = TextureImporterShape.Texture2D;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;

                if (screenshotType == ScreenshotType.Transparent)
                {
                    importer.alphaIsTransparency = true;
                }

                importer.SaveAndReimport();
            }

            var reImportedAsset = AssetImporter.GetAtPath(assetPath);
            EditorGUIUtility.PingObject(reImportedAsset);

            Debug.Log($"Screenshot saved to: {absoluteFilePath}", this);
        }
    }
}
