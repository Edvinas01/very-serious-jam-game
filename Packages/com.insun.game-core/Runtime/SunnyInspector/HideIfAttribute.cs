using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideIfAttribute : SunnyAttribute
    {
        public string MemberName { get; }

        public HideIfAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}
