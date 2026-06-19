using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ButtonAttribute : SunnyAttribute
    {
        public string Text { get; }

        public ButtonAttribute(string text)
        {
            Text = text;
        }
    }
}
