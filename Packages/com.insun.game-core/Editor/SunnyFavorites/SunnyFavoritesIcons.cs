using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyFavorites
{
    internal static class SunnyFavoritesIcons
    {
        private static Texture cachedStarIcon;

        public static Texture StarIcon
        {
            get
            {
                if (cachedStarIcon == false)
                {
                    cachedStarIcon = EditorGUIUtility.IconContent("d_Favorite Icon").image;
                }

                return cachedStarIcon;
            }
        }
    }
}
