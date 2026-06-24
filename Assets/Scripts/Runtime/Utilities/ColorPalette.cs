using System;
using System.Collections.Generic;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Utilities
{
    [SunnySettings(MenuPath = "Colors")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Color Palette Data",
        fileName = "Data_ColorPalette"
    )]
    internal sealed class ColorPalette : ScriptableObject
    {
        [SerializeField]
        private List<ColorEntry> entries;

        public IReadOnlyList<ColorEntry> Entries => entries;

        [Serializable]
        public sealed class ColorEntry
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private Color color;

            public string Name => name;

            public Color Color => color;
        }
    }
}
