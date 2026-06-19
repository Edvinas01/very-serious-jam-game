using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SunnySettingsAttribute : Attribute
    {
        public string MenuPath { get; set; }

        public string NameGetter { get; set; }
    }
}
