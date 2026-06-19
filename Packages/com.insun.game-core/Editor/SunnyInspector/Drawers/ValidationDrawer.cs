using System.Reflection;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEditor;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class ValidationDrawer : AttributeDrawer<ValidateAttribute>
    {
        private MemberInfo validateMember;
        private MemberInfo messageMember;

        public ValidationDrawer(ValidateAttribute attribute, object targetObject) : base(attribute, targetObject)
        {
        }

        public override void Initialize()
        {
            {
                if (ReflectionUtilities.TryGetMember(TargetObject, TypedAttribute.ValidationMember, out var member))
                {
                    validateMember = member;
                }
            }

            {
                if (ReflectionUtilities.TryGetMember(TargetObject, TypedAttribute.MessageMember, out var member))
                {
                    messageMember = member;
                }
            }
        }

        public override void BeginDraw(PropertyDrawer drawer)
        {
            if (ReflectionUtilities.TryGetValue<bool>(TargetObject, validateMember, out var isValid) == false)
            {
                return;
            }

            if (isValid)
            {
                return;
            }

            var messageType = TypedAttribute.ValidationLevel switch
            {
                ValidationLevel.Info => MessageType.Info,
                ValidationLevel.Warning => MessageType.Warning,
                ValidationLevel.Error => MessageType.Error,
                _ => MessageType.None,
            };

            string messageContent;
            if (ReflectionUtilities.TryGetValue<string>(TargetObject, messageMember, out var message))
            {
                messageContent = message;
            }
            else if (TypedAttribute.Message != null)
            {
                messageContent = string.Format(TypedAttribute.Message, drawer.Value);
            }
            else
            {
                messageContent = $"{TargetObject} Failed validation";
            }

            EditorGUILayout.HelpBox(messageContent, messageType);
        }

        public override void EndDraw(PropertyDrawer drawer)
        {
        }
    }
}
