using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyFavorites
{
    internal sealed class SunnyFavoritesData : ScriptableObject
    {
        [SerializeField]
        private List<Object> items = new();

        public IReadOnlyList<Object> Items => items;
    }
}
