using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ColorAttribute : SunnyAttribute
    {
        public float R { get; } = 1.0f;

        public float G { get; } = 1.0f;

        public float B { get; } = 1.0f;

        public float A { get; } = 1.0f;

        public string MemberName { get; }

        public ColorAttribute(float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public ColorAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}
