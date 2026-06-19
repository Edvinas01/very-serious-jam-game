using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnySettings
{
    internal static class SunnySettingsIcons
    {
        private static Texture cachedWindowIcon;
        private static Texture cachedAssetIcon;
        private static Texture cachedCatalogIcon;

        public static Texture SettingsIcon
        {
            get
            {
                if (cachedWindowIcon == false)
                {
                    cachedWindowIcon = EditorGUIUtility.IconContent("SettingsIcon").image;
                }

                return cachedWindowIcon;
            }
        }

        public static Texture ScriptableObjectIcon
        {
            get
            {
                if (cachedAssetIcon == false)
                {
                    cachedAssetIcon = EditorGUIUtility.IconContent("ScriptableObject Icon").image;
                }

                return cachedAssetIcon;
            }
        }

        public static Texture FolderIcon
        {
            get
            {
                if (cachedCatalogIcon == false)
                {
                    cachedCatalogIcon = EditorGUIUtility.IconContent("Folder Icon").image;
                }

                return cachedCatalogIcon;
            }
        }
    }
}
