using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace InSun.GameCore.Editor.Toolbars
{
    internal static class MainToolbarTimescaleSlider
    {
        private static MainToolbarSlider slider;

        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 10f;

        [UsedImplicitly]
        [MainToolbarElement("Timescale/Slider", defaultDockPosition = MainToolbarDockPosition.Middle)]
        public static MainToolbarElement TimeSlider()
        {
            var content = new MainToolbarContent("Time Scale", "Time Scale");

            slider = new MainToolbarSlider(
                content: content,
                value: Time.timeScale,
                minValue: MinTimeScale,
                maxValue: MaxTimeScale,
                valueChanged: OnSliderValueChanged
            );

            slider.populateContextMenu = menu =>
            {
                menu.AppendAction(
                    "Reset",
                    _ =>
                    {
                        Time.timeScale = 1f;
                        MainToolbar.Refresh("Timescale/Slider");
                    }
                );
            };

            slider.enabled = EditorApplication.isPlaying;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            return slider;
        }

        private static void OnSliderValueChanged(float newValue)
        {
            Time.timeScale = newValue;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state is PlayModeStateChange.EnteredPlayMode or PlayModeStateChange.EnteredEditMode)
            {
                MainToolbar.Refresh("Timescale/Slider");
            }
        }
    }
}
