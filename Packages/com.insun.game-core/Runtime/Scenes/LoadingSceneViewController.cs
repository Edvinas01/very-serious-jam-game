using InSun.GameCore.UI;
using UnityEngine;

namespace InSun.GameCore.Scenes
{
    public sealed class LoadingSceneViewController : ViewController<LoadingSceneView>
    {
        public float LoadProgress
        {
            set => View.LoadProgress = value;
        }

        public void Initialize(float showDuration, float hideDuration)
        {
            View.ShowDuration = new Vector2(showDuration, showDuration);
            View.HideDuration = new Vector2(hideDuration, hideDuration);
        }
    }
}
