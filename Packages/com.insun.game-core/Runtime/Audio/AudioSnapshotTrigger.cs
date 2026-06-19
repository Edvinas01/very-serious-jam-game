#if UNITY_AUDIO_INSTALLED
using UnityEngine;
using UnityEngine.Audio;

namespace InSun.GameCore.Audio
{
    public sealed class AudioSnapshotTrigger : MonoBehaviour
    {
        [SerializeField]
        private AudioMixerSnapshot targetSnapshot;

        [Min(0f)]
        [SerializeField]
        private float transitionTime = 0.5f;

        private IAudioSystem audioSystem;

        private void Awake()
        {
            audioSystem = Game.GetObject<IAudioSystem>();
        }

        private void OnDestroy()
        {
            if (audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.DeactivateSnapshot(targetSnapshot, transitionTime);
            }
        }

        [ContextMenu("Switch to Default Snapshot")]
        public void SwitchToDefault()
        {
            if (audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.DeactivateSnapshot(targetSnapshot, transitionTime);
            }
        }

        [ContextMenu("Switch to Target Snapshot")]
        public void SwitchToTarget()
        {
            if (audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.ActivateSnapshot(targetSnapshot, transitionTime);
            }
        }
    }
}

#endif
