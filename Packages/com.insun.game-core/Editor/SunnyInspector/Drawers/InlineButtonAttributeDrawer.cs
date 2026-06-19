using System.Reflection;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class InlineButtonAttributeDrawer : AttributeDrawer<InlineButtonAttribute>
    {
        private MethodInfo buttonMethod;

        public InlineButtonAttributeDrawer(InlineButtonAttribute attribute, object targetObject) : base(attribute, targetObject)
        {
        }

        public override void Initialize()
        {
            var memberName = TypedAttribute.MemberName;
            if (ReflectionUtilities.TryGetMember(TargetObject, memberName, out var member) == false)
            {
                return;
            }

            if (member is MethodInfo method)
            {
                buttonMethod = method;
            }
        }

        public override void BeginDraw(PropertyDrawer drawer)
        {
            if (buttonMethod == null)
            {
                return;
            }

            if (drawer is SerializedPropertyDrawer)
            {
                EditorGUILayout.BeginHorizontal();
            }
        }

        public override void EndDraw(PropertyDrawer drawer)
        {
            if (buttonMethod == null)
            {
                return;
            }

            var isClicked = drawer is SerializedPropertyDrawer
                ? GUILayout.Button(TypedAttribute.Text, GUILayout.Width(128f))
                : GUILayout.Button(TypedAttribute.Text);

            if (isClicked)
            {
                buttonMethod.Invoke(TargetObject, null);
            }

            if (drawer is SerializedPropertyDrawer)
            {
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
