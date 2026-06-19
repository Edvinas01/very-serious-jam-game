namespace InSun.GameCore.Audio
{
    public struct SetAudioParameterArgs
    {
        public IAudioParameter AudioParameter { get; }

        public IAudioEvent AudioEvent { get; set; }

        public float Value { get; set; }

        public SetAudioParameterArgs(IAudioParameter audioParameter)
        {
            AudioParameter = audioParameter;
            AudioEvent = null;
            Value = 0f;
        }
    }
}
