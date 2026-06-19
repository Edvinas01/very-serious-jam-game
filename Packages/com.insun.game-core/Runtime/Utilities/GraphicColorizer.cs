using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InSun.GameCore.Utilities
{
    internal sealed class GraphicColorizer : MonoBehaviour
    {
        [SerializeField]
        private Color color = Color.white;

        [SerializeField]
        private List<Graphic> graphics;

        public void ApplyColor()
        {
            if (isActiveAndEnabled == false)
            {
                return;
            }

            foreach (var graphic in graphics)
            {
                graphic.color = color;
            }
        }
    }
}
