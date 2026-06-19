#if UNITY_AUDIO_INSTALLED
using System.Collections.Generic;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    public sealed class PooledAudioSource : MonoBehaviour
    {
        [SerializeField]
        private bool isFallbackToRandomSource;

        [SerializeField]
        private List<AudioSourceExtended> sources;

        public void Play()
        {
            foreach (var source in sources)
            {
                if (source.IsPlaying == false)
                {
                    source.Play();
                    return;
                }
            }

            if (isFallbackToRandomSource)
            {
                if (sources.TryGetRandom(out var source))
                {
                    source.Play();
                }
            }
        }
    }
}

#endif
