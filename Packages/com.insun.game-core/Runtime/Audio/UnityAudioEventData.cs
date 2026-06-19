#if UNITY_AUDIO_INSTALLED

using System.Collections.Generic;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;
using UnityEngine.Audio;

namespace InSun.GameCore.Audio
{
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/Audio/Unity Audio Event",
        fileName = MenuConstants.BaseAssetFileName + "Data_AudioEvent"
    )]
    internal sealed class UnityAudioEventData : AudioEventData
    {
        [Header("General")]
        [SerializeField]
        private AudioMixerGroup mixerGroup;

        [SerializeField]
        private List<AudioClip> audioClips;

        [Header("Randomization")]
        [Min(0f)]
        [SerializeField]
        private Vector2 volumeRange = new(1.0f, 1.0f);

        [Min(0f)]
        [SerializeField]
        private Vector2 pitchRange = new(1.0f, 1.0f);

        [Min(0f)]
        [SerializeField]
        private Vector2 playTimeRange = new(0f, 0f);

        [SerializeField]
        private Vector2 panningRange = new(0f, 0f);

        [Header("Instances")]
        [Tooltip("Global count for instances of THIS event")]
        [Min(0)]
        [SerializeField]
        private int maxInstanceCount = 10;

        [Tooltip(
            ""
            + "If an emitter reaches the global limit, it will try to steal the oldest audio "
            + "source instead of doing nothing"
        )]
        [SerializeField]
        private bool isStealing;

        [Header("Playing")]
        [SerializeField]
        [Min(0f)]
        private float playCooldown;

        [SerializeField]
        [Range(0f, 1f)]
        private float playChance = 1f;

        [SerializeField]
        private bool isLoop;

        [Header("Fade in & out")]
        [SerializeField]
        private bool isFadeInOnPlay;

        [ShowIf(nameof(isFadeInOnPlay))]
        [Min(0f)]
        [SerializeField]
        private float fadeInDuration = 0.5f;

        [SerializeField]
        private bool isFadeOutOnStop;

        [ShowIf(nameof(isFadeOutOnStop))]
        [Min(0f)]
        [SerializeField]
        private float fadeOutDuration = 0.5f;

        [Header("3D")]
        [Range(0f, 1f)]
        [SerializeField]
        private float spatialBlend;

        [ShowIf(nameof(IsShow3DAudioEditor))]
        [SerializeField]
        private AudioRolloffMode audioRolloffMode = AudioRolloffMode.Logarithmic;

        [ShowIf(nameof(IsShow3DAudioCustomRolloffEditor))]
        [SerializeField]
        private AnimationCurve customRolloffCurve;

        [ShowIf(nameof(IsShow3DAudioEditor))]
        [Min(0f)]
        [SerializeField]
        private Vector2 distanceRange = new(1f, 30f);

        public int MaxInstanceCount => maxInstanceCount;

        public override string Name => name;

        public AudioMixerGroup MixerGroup => mixerGroup;

        public IReadOnlyList<AudioClip> AudioClips => audioClips;

        public Vector2 VolumeRange => volumeRange;

        public Vector2 PitchRange => pitchRange;

        public Vector2 PlayTimeRange => playTimeRange;

        public Vector2 PanningRange => panningRange;

        public bool IsFadeInOnPlay => isFadeInOnPlay;

        public float FadeInDuration => fadeInDuration;

        public bool IsFadeOutOnStop => isFadeOutOnStop;

        public float FadeOutDuration => fadeOutDuration;

        public float SpatialBlend => spatialBlend;

        public AnimationCurve CustomRolloffCurve => customRolloffCurve;

        public AudioRolloffMode AudioRolloffMode => audioRolloffMode;

        public override Vector2 DistanceRange => distanceRange;

        public float PlayCooldown => playCooldown;

        public float PlayChance => playChance;

        public bool IsLoop => isLoop;

        public bool IsStealing => isStealing;

        private bool IsShow3DAudioCustomRolloffEditor()
        {
            return IsShow3DAudioEditor() && audioRolloffMode == AudioRolloffMode.Custom;
        }

        private bool IsShow3DAudioEditor()
        {
            return spatialBlend > 0f;
        }
    }
}

#endif
