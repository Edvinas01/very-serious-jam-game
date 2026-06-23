using System;
using System.Collections.Generic;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Audio
{
    [Serializable]
    internal sealed class AudioData
    {
        [Header("Assets")]
        [SerializeField]
        private List<AudioClip> clips;

        [Header("Features")]
        [SerializeField]
        private Vector2 pitchRange = new(1f, 1f);

        [SerializeField]
        private Vector2 volumeRange = new(1f, 1f);

        [SerializeField]
        private bool isLooping;

        [Header("Fade")]
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

        public IReadOnlyList<AudioClip> Clips => clips;

        public Vector2 PitchRange => pitchRange;

        public Vector2 VolumeRange => volumeRange;

        public bool IsLooping => isLooping;

        public bool IsFadeInOnPlay => isFadeInOnPlay;

        public float FadeInDuration => fadeInDuration;

        public bool IsFadeOutOnStop => isFadeOutOnStop;

        public float FadeOutDuration => fadeOutDuration;
    }
}
