using InSun.GameCore.SunnyInspector;

namespace InSun.GameCore.Editor.SunnyInspector
{
    internal abstract class AttributeDrawer<T> : AttributeDrawer where T : SunnyAttribute
    {
        public T TypedAttribute { get; }

        protected AttributeDrawer(T attribute, object targetObject) : base(attribute, targetObject)
        {
            TypedAttribute = attribute;
        }
    }

    internal abstract class AttributeDrawer
    {
        public SunnyAttribute Attribute { get; }

        public object TargetObject { get; }

        protected AttributeDrawer(SunnyAttribute attribute, object targetObject)
        {
            Attribute = attribute;
            TargetObject = targetObject;
        }

        public abstract void Initialize();

        public abstract void BeginDraw(PropertyDrawer drawer);

        public abstract void EndDraw(PropertyDrawer drawer);
    }
}
