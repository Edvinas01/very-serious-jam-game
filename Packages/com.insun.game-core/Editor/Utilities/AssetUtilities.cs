using System.IO;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.Utilities
{
    /// <summary>
    /// Utilities for creating editor assets.
    /// </summary>
    public static class AssetUtilities
    {
        /// <summary>
        /// Create a new asset at given <paramref name="path"/>.
        /// </summary>
        /// <returns>
        /// Created asset.
        /// </returns>
        public static T CreateAsset<T>(
            string path,
            bool isCreateDirectory = true,
            bool isRefresh = true,
            bool isPing = true
        ) where T : ScriptableObject
        {
#if UNITY_EDITOR
            var scriptableObject = ScriptableObject.CreateInstance<T>();

            SaveAsset(
                scriptableObject,
                path,
                isCreateDirectory,
                isRefresh,
                isPing
            );

            return scriptableObject;
#else
            return null;
#endif
        }

        /// <summary>
        /// Save <paramref name="asset"/> at given <paramref name="path"/>.
        /// </summary>
        public static void SaveAsset(
            Object asset,
            string path,
            bool isCreateDirectory = true,
            bool isRefresh = true,
            bool isPing = true
        )
        {
#if UNITY_EDITOR
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (isCreateDirectory)
            {
                CreateDirectory(path);
            }

            var uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{path}.asset");
            AssetDatabase.CreateAsset(asset, uniquePath);
            AssetDatabase.SaveAssets();

            if (isRefresh)
            {
                AssetDatabase.Refresh();
            }

            if (isPing)
            {
                EditorGUIUtility.PingObject(asset);
            }
#endif
        }

        private static void CreateDirectory(string path)
        {
#if UNITY_EDITOR
            var directoryPath = Path.GetDirectoryName(path);
            if (directoryPath == null || Directory.Exists(directoryPath))
            {
                return;
            }

            Directory.CreateDirectory(directoryPath);
            AssetDatabase.Refresh();
#endif
        }
    }
}
