namespace InSun.GameCore.Audio
{
    public interface IAudioSystem
    {
        public bool IsLoading { get; }

        public void Load();

        public void UnLoad();

        public float GetParameter(GetAudioParameterArgs args);

        public void SetParameter(SetAudioParameterArgs args);

        public IAudioInstance Play(PlayAudioEventArgs args);
    }
}
