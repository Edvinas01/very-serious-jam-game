using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public static class GizmoUtilities
    {
#if UNITY_EDITOR
        private static GUIStyle labelStyle;
        private static Texture2D labelBgTex;

        private static GUIStyle LabelStyle
        {
            get
            {
                if (labelStyle != null)
                {
                    return labelStyle;
                }

                labelBgTex = new Texture2D(1, 1);
                labelBgTex.SetPixel(0, 0, Color.black);
                labelBgTex.Apply();

                labelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                {
                    normal =
                    {
                        textColor = Color.cyan,
                        background = labelBgTex,
                    },
                    padding = new RectOffset(4, 4, 2, 2),
                };

                return labelStyle;
            }
        }
#endif

        public static void DrawLabel(
            Vector3 position,
            string text,
            int fontSize = 12,
            Vector2? distanceRange = null,
            Color? color = null
        )
        {
#if UNITY_EDITOR
            var distanceToCamera = GetDistanceToCamera(position);
            var progress = Mathf.InverseLerp(distanceRange?.y ?? 7f, distanceRange?.x ?? 3f, distanceToCamera);
            var textColor = color ?? Color.white;
            textColor.a = progress;

            DrawLabel(position, text, textColor, fontSize);
#endif
        }

        public static void DrawLabel(
            Vector3 position,
            string text,
            Color color,
            int fontSize = 12
        )
        {
#if UNITY_EDITOR
            var style = new GUIStyle(LabelStyle)
            {
                fontSize = fontSize,
                normal =
                {
                    textColor = color,
                },
            };

            var prevGuiColor = GUI.color;
            GUI.color = color;

            UnityEditor.Handles.Label(position, text, style);
            GUI.color = prevGuiColor;
#endif
        }

        public static void DrawLabels(IEnumerable<(Vector3 position, string text)> labels)
        {
#if UNITY_EDITOR
            using (new UnityEditor.Handles.DrawingScope())
            {
                foreach (var (position, text) in labels)
                {
                    UnityEditor.Handles.Label(position, text, LabelStyle);
                }
            }
#endif
        }

        private static float GetDistanceToCamera(Vector3 labelPosition)
        {
#if UNITY_EDITOR
            Camera camera;
            if (Application.isPlaying && UnityEditor.EditorWindow.focusedWindow is not UnityEditor.SceneView)
            {
                camera = Camera.main;
            }
            else
            {
                camera = UnityEditor.SceneView.lastActiveSceneView.camera;
            }

            if (camera == null)
            {
                return 0f;
            }

            var cameraPosition = camera.transform.position;
            return Vector3.Distance(cameraPosition, labelPosition);
#else
            return 0f;
#endif
        }
    }
}
