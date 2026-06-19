using System;

namespace InSun.GameCore.SunnyInspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public sealed class ValidateAttribute : SunnyAttribute
    {
        public string Message { get; set; }

        public string MessageMember { get; set; }

        public string ValidationMember { get; }

        public ValidationLevel ValidationLevel { get; set; }

        public ValidateAttribute(string validationMember, string messageMember = null, ValidationLevel validationLevel = ValidationLevel.Warning)
        {
            ValidationMember = validationMember;
            MessageMember = messageMember;
            ValidationLevel = validationLevel;
        }
    }
}
