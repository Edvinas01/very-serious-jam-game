using System.Collections.Generic;

namespace InSun.GameCore.Editor.SunnyInspector
{
    internal abstract class PropertyDrawer
    {
        private readonly List<AttributeDrawer> attributes;
        private readonly List<PropertyDrawer> children;

        public abstract string DisplayName { get; }

        public abstract int Depth { get; }

        public abstract object Value { get; }

        public PropertyDrawer Parent { get; }

        public IReadOnlyCollection<AttributeDrawer> Attributes => attributes;

        public IReadOnlyCollection<PropertyDrawer> Children => children;

        public virtual void Initialize()
        {
            foreach (var attribute in attributes)
            {
                attribute.Initialize();
            }
        }

        public abstract void Draw();

        protected PropertyDrawer(PropertyDrawer parent, List<AttributeDrawer> attributes, List<PropertyDrawer> children)
        {
            this.attributes = attributes;
            this.children = children;
            Parent = parent;
        }

        public void AddChild(PropertyDrawer node)
        {
            children.Add(node);
        }
    }
}
