using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyScreenshots
{
    internal static class SunnyScreenshotsIcons
    {
        private static Texture cachedCameraIcon;

        public static Texture CameraIcon
        {
            get
            {
                if (cachedCameraIcon == false)
                {
                    cachedCameraIcon = EditorGUIUtility.IconContent("Camera Gizmo").image;
                }

                return cachedCameraIcon;
            }
        }
    }
}
