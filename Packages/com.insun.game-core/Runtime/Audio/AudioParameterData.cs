using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    [SunnySettings(MenuPath = "Audio")]
    public abstract class AudioParameterData : ScriptableObject, IAudioParameter
    {
        public abstract string Name { get; }
    }
}
