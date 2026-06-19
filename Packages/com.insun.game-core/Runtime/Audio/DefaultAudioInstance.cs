using System;

namespace InSun.GameCore.Audio
{
    internal sealed class DefaultAudioInstance : IAudioInstance
    {
        public static readonly DefaultAudioInstance Instance = new();

        public bool IsPlaying => false;

        public event Action<IAudioInstance> OnStopped
        {
            add { }
            remove { }
        }

        private DefaultAudioInstance()
        {
        }

        public void Play()
        {
        }

        public void Stop()
        {
        }
    }
}
