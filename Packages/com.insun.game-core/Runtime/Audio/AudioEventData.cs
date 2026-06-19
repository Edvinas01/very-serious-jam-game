using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    [SunnySettings(MenuPath = "Audio")]
    public abstract class AudioEventData : ScriptableObject, IAudioEvent
    {
        public abstract Vector2 DistanceRange { get; }

        public abstract string Name { get; }
    }
}
