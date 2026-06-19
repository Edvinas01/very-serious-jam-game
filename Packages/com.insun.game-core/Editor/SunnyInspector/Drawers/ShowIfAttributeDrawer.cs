using System.Reflection;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class ShowIfAttributeDrawer : AttributeDrawer<ShowIfAttribute>
    {
        private MemberInfo showIfMember;

        public bool IsVisible
        {
            get
            {
                if (ReflectionUtilities.TryGetValue<bool>(TargetObject, showIfMember, out var isVisible))
                {
                    return isVisible;
                }

                if (ReflectionUtilities.TryGetValue<object>(TargetObject, showIfMember, out var obj))
                {
                    if (obj is Object unityObject)
                    {
                        return unityObject;
                    }

                    return obj != null;
                }

                return false;
            }
        }

        public ShowIfAttributeDrawer(ShowIfAttribute attribute, object targetObject) : base(attribute, targetObject)
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
