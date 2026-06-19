using System.Reflection;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class HideIfAttributeDrawer : AttributeDrawer<HideIfAttribute>
    {
        private MemberInfo showIfMember;

        public bool IsHidden
        {
            get
            {
                if (ReflectionUtilities.TryGetValue<bool>(TargetObject, showIfMember, out var isHidden))
                {
                    return isHidden;
                }

                if (ReflectionUtilities.TryGetValue<object>(TargetObject, showIfMember, out var obj))
                {
                    if (obj is Object unityObject)
                    {
                        return unityObject;
                    }
                }

                return false;
            }
        }

        public HideIfAttributeDrawer(HideIfAttribute attribute, object targetObject) : base(attribute, targetObject)
        {
        }

        public override void Initialize()
        {
            if (ReflectionUtilities.TryGetMember(TargetObject, TypedAttribute.MemberName, out var member))
            {
                showIfMember = member;
            }
        }

        public override void BeginDraw(PropertyDrawer drawer)
        {
        }

        public override void EndDraw(PropertyDrawer drawer)
        {
        }
    }
}
