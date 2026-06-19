using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ShowIfAttribute : SunnyAttribute
    {
        public string MemberName { get; }

        public ShowIfAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}
