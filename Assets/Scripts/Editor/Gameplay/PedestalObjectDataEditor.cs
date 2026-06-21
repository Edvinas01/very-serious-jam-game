using System.IO;
using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore.Editor.SunnyInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoubleD.VerySeriousJamGame.Editor.Gameplay
{
    [CustomEditor(typeof(PaintableData))]
    internal sealed class PedestalObjectDataEditor : SunnyEditor
    {
        private const int TextureSize = 128;
        private const string ScreenshotScenePath = "Assets/Scenes/Scene_SpriteScreenshots.unity";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Icon Sprite"))
            {
                GenerateIconSprite();
            }
        }

        private void GenerateIconSprite()
        {
            var data = (PaintableData)target;

            var (assetPath, fullPath) = GetSpritePath(data);

            var previousScenes = new string[SceneManager.sceneCount];
            for (var index = 0; index < SceneManager.sceneCount; index++)
            {
                var previousScene = SceneManager.GetSceneAt(index);
                previousScenes[index] = previousScene.path;
            }

            var screenshotScene = EditorSceneManager.OpenScene(ScreenshotScenePath, OpenSceneMode.Single);
            var spriteTexture = RenderSprite(data.Prefab, screenshotScene);
            var spriteBytes = spriteTexture.EncodeToPNG();
            DestroyImmediate(spriteTexture);

            var isFirstScene = true;
            foreach (var path in previousScenes)
            {
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                EditorSceneManager.OpenScene(path, isFirstScene ? OpenSceneMode.Single : OpenSceneMode.Additive);
                isFirstScene = false;
            }

            ApplySprite(spriteBytes, assetPath, fullPath);
        }

        private (string assetPath, string fullPath) GetSpritePath(PaintableData data)
        {
            var safeName = string.IsNullOrWhiteSpace(data.Name) ? target.name : data.Name.Replace(" ", "_");
            var spriteName = $"PedestalObject_Icon_{safeName}";
            var assetPath = $"Assets/Sprites/PedestalObjects/{spriteName}.png";
            var fullDirPath = Path.Combine(Application.dataPath, "Sprites", "PedestalObjects");

            if (Directory.Exists(fullDirPath) == false)
            {
                Directory.CreateDirectory(fullDirPath);
            }

            return (assetPath, Path.Combine(fullDirPath, $"{spriteName}.png"));
        }

        private static Texture2D RenderSprite(GameObject prefab, Scene scene)
        {
            var camera = GetCameraInScene(scene);
            var previewGameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            previewGameObject.transform.position = Vector3.zero;

            var renderTexture = new RenderTexture(
                width: TextureSize,
                height: TextureSize,
                depth: 16,
                format: RenderTextureFormat.ARGB32
            );
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = null;

            RenderTexture.active = renderTexture;
            var finalTexture = new Texture2D(
                width: TextureSize,
                height: TextureSize,
                textureFormat: TextureFormat.RGBA32,
                mipChain: false
            );
            finalTexture.ReadPixels(new Rect(0, 0, TextureSize, TextureSize), 0, 0);
            finalTexture.Apply();

            RenderTexture.active = null;

            DestroyImmediate(previewGameObject);
            renderTexture.Release();

            DestroyImmediate(renderTexture);

            return finalTexture;
        }

        private void ApplySprite(byte[] pngBytes, string assetPath, string fullPath)
        {
            File.WriteAllBytes(fullPath, pngBytes);

            AssetDatabase.Refresh();

            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();

            var objectSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            var iconProp = serializedObject.FindProperty("icon");
            iconProp.objectReferenceValue = objectSprite;
            serializedObject.ApplyModifiedProperties();

            Debug.Log($"Generated sprite at: '{assetPath}'", objectSprite);

            EditorGUIUtility.PingObject(objectSprite);
        }

        private static Camera GetCameraInScene(Scene scene)
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var camera = root.GetComponentInChildren<Camera>(true);
                if (camera)
                {
                    return camera;
                }
            }

            return null;
        }
    }
}
