using InSun.GameCore.Utilities;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Audio
{
    internal static class AudioSourceExtensions
    {
        public static void PlayUsing(this AudioSource source, AudioData audioData)
        {
            if (source == false)
            {
                return;
            }

            if (audioData == null)
            {
                return;
            }

            source.Stop();

            if (audioData.Clips.TryGetRandom(out var clip))
            {
                source.clip = clip;
            }

            source.loop = audioData.IsLooping;
            source.volume = audioData.VolumeRange.GetRandomFloat();
            source.pitch = audioData.PitchRange.GetRandomFloat();
            source.Play();
        }
    }
}
