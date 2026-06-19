using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyFavorites
{
    internal sealed class SunnyFavoritesWindow : EditorWindow
    {
        private static readonly string DataPathKey = typeof(SunnyFavoritesWindow).FullName + ".DataPath";

        [SerializeField]
        private SunnyFavoritesData data;

        private UnityEditor.Editor currentEditor;

        [MenuItem(
            MenuConstants.BaseWindowMenuName + "/Sunny Favorites",
            priority = MenuConstants.BaseWindowMenuPriority
        )]
        public static void OpenWindow()
        {
            var window = GetWindow<SunnyFavoritesWindow>();
            window.titleContent = new GUIContent("Sunny Favorites", SunnyFavoritesIcons.StarIcon);
            window.minSize = new Vector2(200, 100);
            window.Show();
        }

        private void OnEnable()
        {
            TryLoadDataFromPrefs();
            RebuildDataEditor();
        }

        private void OnDisable()
        {
            DestroyDataEditor();
        }

        private void OnGUI()
        {
            DrawDataField();

            if (data == false)
            {
                return;
            }

            if (currentEditor == false)
            {
                RebuildDataEditor();
            }

            EditorGUILayout.Space(4f);
            currentEditor.OnInspectorGUI();
        }

        private void DrawDataField()
        {
            EditorGUI.BeginChangeCheck();

            data = (SunnyFavoritesData)EditorGUILayout.ObjectField(
                data,
                typeof(SunnyFavoritesData),
                allowSceneObjects: false
            );

            if (EditorGUI.EndChangeCheck())
            {
                OnDataChanged();
            }

            if (data == false)
            {
                if (GUILayout.Button("Create New"))
                {
                    TryCreateData();
                }
            }
        }

        private void OnDataChanged()
        {
            if (data)
            {
                EditorPrefs.SetString(DataPathKey, AssetDatabase.GetAssetPath(data));
            }
            else
            {
                EditorPrefs.DeleteKey(DataPathKey);
            }

            RebuildDataEditor();
        }

        private void TryLoadDataFromPrefs()
        {
            if (data)
            {
                return;
            }

            var path = EditorPrefs.GetString(DataPathKey, string.Empty);
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            data = AssetDatabase.LoadAssetAtPath<SunnyFavoritesData>(path);
        }

        private void TryCreateData()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                title: "Create Favorites Data",
                defaultName: nameof(SunnyFavoritesData),
                extension: "asset",
                message: "Choose where to save the Favorites data asset"
            );

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var asset = CreateInstance<SunnyFavoritesData>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();

            data = asset;
            OnDataChanged();
        }

        private void RebuildDataEditor()
        {
            DestroyDataEditor();

            if (data == false)
            {
                return;
            }

            currentEditor = UnityEditor.Editor.CreateEditor(data);
        }

        private void DestroyDataEditor()
        {
            if (currentEditor == false)
            {
                return;
            }

            DestroyImmediate(currentEditor);
            currentEditor = null;
        }
    }
}
