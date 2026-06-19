using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InlineButtonAttribute : SunnyAttribute
    {
        public string MemberName { get; }

        public string Text { get; }

        public InlineButtonAttribute(string text, string memberName)
        {
            Text = text;
            MemberName = memberName;
        }
    }
}
