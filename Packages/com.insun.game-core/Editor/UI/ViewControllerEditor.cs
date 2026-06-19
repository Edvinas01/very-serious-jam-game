using InSun.GameCore.Editor.SunnyInspector;
using InSun.GameCore.UI;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.UI
{
    [CustomEditor(typeof(ViewController), true)]
    internal sealed class ViewControllerEditor : SunnyEditor
    {
        private ViewController viewController;

        protected override void OnEnable()
        {
            if (target == false)
            {
                return;
            }

            base.OnEnable();

            viewController = (ViewController)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controls", EditorStyles.label);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(disabled: Application.isPlaying))
                {
                    if (GUILayout.Button("Init"))
                    {
                        viewController.InitializeView();
                    }
                }

                using (new EditorGUI.DisabledScope(disabled: Application.isPlaying == false))
                {
                    if (GUILayout.Button("Show"))
                    {
                        viewController.ShowView();
                    }

                    if (GUILayout.Button("Hide"))
                    {
                        viewController.HideView();
                    }
                }
            }
        }
    }
}
