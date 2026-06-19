using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LabelAttribute : SunnyAttribute
    {
        public string Text { get; }

        public LabelAttribute(string text)
        {
            Text = text;
        }
    }
}
