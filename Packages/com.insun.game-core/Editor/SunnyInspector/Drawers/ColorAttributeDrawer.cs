using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class ColorAttributeDrawer : AttributeDrawer<ColorAttribute>
    {
        private Color prevColor;
        private Color color;

        public ColorAttributeDrawer(ColorAttribute attribute, object targetObject) : base(attribute, targetObject)
        {
        }

        public override void Initialize()
        {
            color = new Color(TypedAttribute.R, TypedAttribute.G, TypedAttribute.B, TypedAttribute.A);

            var memberName = TypedAttribute.MemberName;
            if (string.IsNullOrWhiteSpace(memberName))
            {
                return;
            }

            if (ReflectionUtilities.TryGetMember(TargetObject, memberName, out var member) == false)
            {
                return;
            }

            if (ReflectionUtilities.TryGetValue<Color>(TargetObject, member, out var memberColor))
            {
                color = memberColor;
            }
        }

        public override void BeginDraw(PropertyDrawer drawer)
        {
            prevColor = GUI.color;
            GUI.color = color;
        }

        public override void EndDraw(PropertyDrawer drawer)
        {
            GUI.color = prevColor;
        }
    }
}
