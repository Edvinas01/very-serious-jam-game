using InSun.GameCore.SunnyInspector;

namespace InSun.GameCore.Editor.SunnyInspector.Drawers
{
    internal sealed class LabelAttributeDrawer : AttributeDrawer<LabelAttribute>
    {
        public string LabelText => TypedAttribute.Text;

        public LabelAttributeDrawer(LabelAttribute attribute, object targetObject) : base(attribute, targetObject)
        {
        }

        public override void Initialize()
        {
        }

        public override void BeginDraw(PropertyDrawer drawer)
        {
        }

        public override void EndDraw(PropertyDrawer drawer)
        {
        }
    }
}
