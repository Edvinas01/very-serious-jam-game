using System;
using System.Collections.Generic;
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

        public IReadOnlyList<AudioClip> Clips => clips;

        public Vector2 PitchRange => pitchRange;

        public Vector2 VolumeRange => volumeRange;
    }
}
