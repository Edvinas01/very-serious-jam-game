namespace InSun.GameCore.Audio
{
    public struct GetAudioParameterArgs
    {
        public IAudioParameter AudioParameter { get; }

        public IAudioEvent AudioEvent { get; set; }

        public GetAudioParameterArgs(IAudioParameter audioParameter)
        {
            AudioParameter = audioParameter;
            AudioEvent = null;
        }
    }
}
