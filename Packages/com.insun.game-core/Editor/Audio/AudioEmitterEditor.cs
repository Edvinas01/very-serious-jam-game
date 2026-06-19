using InSun.GameCore.Audio;
using InSun.GameCore.Editor.SunnyInspector;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.Audio
{
    [CustomEditor(typeof(AudioEmitter))]
    internal sealed class AudioEmitterEditor : SunnyEditor
    {
        private bool isAudioInstancesExpanded;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var emitter = (AudioEmitter)target;
            using (new EditorGUI.DisabledGroupScope(disabled: Application.isPlaying == false))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

                using (new EditorGUI.DisabledGroupScope(disabled: true))
                {
                    EditorGUILayout.Toggle("IsPlaying", emitter.IsPlaying);
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Play"))
                    {
                        emitter.Play();
                    }

                    if (GUILayout.Button("Stop"))
                    {
                        emitter.Stop();
                    }
                }

                isAudioInstancesExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(
                    foldout: isAudioInstancesExpanded,
                    content: $"Audio Instances ({emitter.ActiveInstances.Count})"
                );

                if (isAudioInstancesExpanded)
                {
                    using (new EditorGUI.DisabledGroupScope(disabled: true))
                    {
                        foreach (var audioInstance in emitter.ActiveInstances)
                        {
                            if (audioInstance is Object instanceObj)
                            {
                                EditorGUILayout.ObjectField(
                                    label: $"Instance (playing={audioInstance.IsPlaying})",
                                    obj: instanceObj,
                                    objType: typeof(IAudioInstance),
                                    allowSceneObjects: false
                                );
                            }
                        }
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
    }
}
