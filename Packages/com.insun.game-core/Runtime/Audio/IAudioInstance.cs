using System;

namespace InSun.GameCore.Audio
{
    public interface IAudioInstance
    {
        public bool IsPlaying { get; }

        public event Action<IAudioInstance> OnStopped;

        public void Play();

        public void Stop();
    }
}
