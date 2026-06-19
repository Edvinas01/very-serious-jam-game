using System.Collections.Generic;
using InSun.GameCore.Editor.SunnyInspector.Drawers;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector
{
    internal sealed class SerializedPropertyDrawer : PropertyDrawer
    {
        public override string DisplayName => SerializedProperty.displayName;

        public override int Depth => SerializedProperty.depth;

        public override object Value => SerializedProperty.boxedValue;

        public SerializedProperty SerializedProperty { get; }

        public SerializedPropertyDrawer(
            SerializedProperty serializedProperty,
            PropertyDrawer parent,
            List<AttributeDrawer> attributes,
            List<PropertyDrawer> children
        ) : base(parent, attributes, children)
        {
            SerializedProperty = serializedProperty;
        }

        public override void Draw()
        {
            DrawPropertyRecursive(this);
        }

        private static void DrawPropertyRecursive(SerializedPropertyDrawer drawer)
        {
            if (IsVisible(drawer) == false)
            {
                return;
            }

            if (drawer.SerializedProperty.name.Equals("m_Script"))
            {
                using (new EditorGUI.DisabledScope(disabled: true))
                {
                    DrawProperty(drawer, includeChildren: true);
                }

                return;
            }

            if (drawer.SerializedProperty.isArray)
            {
                DrawProperty(drawer, includeChildren: true);
                return;
            }

            DrawProperty(drawer, includeChildren: false);

            if (drawer.SerializedProperty.isExpanded == false)
            {
                return;
            }

            EditorGUI.indentLevel++;

            try
            {
                foreach (var child in drawer.Children)
                {
                    child.Draw();
                }
            }
            finally
            {
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawProperty(SerializedPropertyDrawer drawer, bool includeChildren)
        {
            foreach (var attribute in drawer.Attributes)
            {
                attribute.BeginDraw(drawer);
            }

            EditorGUILayout.PropertyField(
                property: drawer.SerializedProperty,
                label: GetLabelContent(drawer),
                includeChildren: includeChildren
            );

            foreach (var attribute in drawer.Attributes)
            {
                attribute.EndDraw(drawer);
            }
        }

        private static bool IsVisible(PropertyDrawer drawer)
        {
            foreach (var propertyAttribute in drawer.Attributes)
            {
                if (propertyAttribute is ShowIfAttributeDrawer showIfDrawer)
                {
                    return showIfDrawer.IsVisible;
                }

                if (propertyAttribute is HideIfAttributeDrawer hideIfDrawer)
                {
                    return hideIfDrawer.IsHidden == false;
                }
            }

            return true;
        }

        private static GUIContent GetLabelContent(PropertyDrawer drawer)
        {
            foreach (var propertyAttribute in drawer.Attributes)
            {
                if (propertyAttribute is LabelAttributeDrawer labelDrawer)
                {
                    return new GUIContent(labelDrawer.LabelText);
                }
            }

            return new GUIContent(drawer.DisplayName);
        }
    }
}
