using InSun.GameCore.UI;
using TMPro;
using UnityEngine;

namespace InSun.GameCore.Scenes
{
    public sealed class LoadingSceneView : View
    {
        [SerializeField]
        private TMP_Text loadingText;

        [SerializeField]
        private string zeroProgressLoadingTextFormat = "Loading...";

        [SerializeField]
        private string loadingTextFormat = "Loading {0:0%}";

        public float LoadProgress
        {
            set
            {
                if (loadingText)
                {
                    if (value <= 0f)
                    {
                        loadingText.text = string.Format(zeroProgressLoadingTextFormat, value);
                    }
                    else
                    {
                        loadingText.text = string.Format(loadingTextFormat, value);
                    }
                }
            }
        }

        public Vector2 ShowDuration
        {
            set
            {
                if (ShowAnimation)
                {
                    ShowAnimation.Duration = value;
                }
            }
        }

        public Vector2 HideDuration
        {
            set
            {
                if (HideAnimation)
                {
                    HideAnimation.Duration = value;
                }
            }
        }
    }
}
