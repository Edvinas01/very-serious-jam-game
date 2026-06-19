using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyWebGL
{
    internal static class SunnyWebGLIcons
    {
        private static Texture cachedWebGLIcon;

        public static Texture WebGLIcon
        {
            get
            {
                if (cachedWebGLIcon == false)
                {
                    cachedWebGLIcon = EditorGUIUtility.IconContent("d_BuildSettings.WebGL@2x").image;
                }

                return cachedWebGLIcon;
            }
        }
    }
}
