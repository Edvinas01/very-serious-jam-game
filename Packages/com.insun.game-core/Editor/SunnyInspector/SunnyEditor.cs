using System.Collections.Generic;
using System.Linq;
using InSun.GameCore.Editor.SunnyInspector.Drawers;
using InSun.GameCore.Editor.SunnyInspector.Utilities;
using InSun.GameCore.SunnyInspector;
using UnityEditor;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyInspector
{
#if !ODIN_INSPECTOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    internal sealed class SunnyScriptableObjectEditor : SunnyEditor
    {
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), true)]
    internal sealed class SunnyMonoBehaviourEditor : SunnyEditor
    {
    }
#endif

    public abstract class SunnyEditor : UnityEditor.Editor
    {
        private readonly List<PropertyDrawer> propertyDrawers = new();
        private bool isAnySunnyAttribute;

        protected virtual void OnEnable()
        {
            if (target == false)
            {
                return;
            }

            InitializePropertyDrawers();
        }

        protected virtual void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            if (isAnySunnyAttribute)
            {
                DrawPropertyDrawers();
            }
            else
            {
                DrawDefaultInspector();
            }
        }

        private void InitializePropertyDrawers()
        {
            propertyDrawers.Clear();

            var stack = new Stack<PropertyDrawer>();

            using var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (string.IsNullOrEmpty(iterator.propertyPath))
                {
                    continue;
                }

                var serializedProperty = serializedObject.FindProperty(iterator.propertyPath);
                while (stack.Count > 0 && stack.Peek().Depth >= serializedProperty.depth)
                {
                    // Pop until we find a parent with smaller depth
                    stack.Pop();
                }

                var parentPropertyDrawer = stack.Count > 0 ? stack.Peek() : null;
                var propertyDrawer = CreatePropertyDrawer(serializedProperty, parentPropertyDrawer);
                if (propertyDrawer.Attributes.Count > 0)
                {
                    isAnySunnyAttribute = true;
                }

                propertyDrawer.Initialize();

                if (parentPropertyDrawer != null)
                {
                    parentPropertyDrawer.AddChild(propertyDrawer);
                }
                else
                {
                    propertyDrawers.Add(propertyDrawer);
                }

                stack.Push(propertyDrawer);
            }
        }

        private PropertyDrawer CreatePropertyDrawer(SerializedProperty serializedProperty, PropertyDrawer parent)
        {
            return new SerializedPropertyDrawer(
                serializedProperty: serializedProperty,
                parent: parent,
                attributes: CreateAttributeDrawers(serializedProperty),
                children: new List<PropertyDrawer>()
            );
        }

        private void DrawPropertyDrawers()
        {
            if (propertyDrawers.Count <= 0)
            {
                return;
            }

            serializedObject.Update();

            foreach (var propertyNode in propertyDrawers)
            {
                propertyNode.Draw();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static List<AttributeDrawer> CreateAttributeDrawers(SerializedProperty serializedProperty)
        {
            var targetObject = PropertyUtilities.GetTargetObjectWithProperty(serializedProperty);

            return PropertyUtilities
                .GetAttributes<SunnyAttribute>(serializedProperty)
                .Select(attribute => CreateAttributeDrawer(attribute, targetObject))
                .Where(attribute => attribute != null)
                .ToList();
        }

        private static AttributeDrawer CreateAttributeDrawer(SunnyAttribute attribute, object targetObject)
        {
            if (attribute is ColorAttribute colorAttribute)
            {
                return new ColorAttributeDrawer(colorAttribute, targetObject);
            }

            if (attribute is ShowIfAttribute showIfAttribute)
            {
                return new ShowIfAttributeDrawer(showIfAttribute, targetObject);
            }

            if (attribute is HideIfAttribute hideIfAttribute)
            {
                return new HideIfAttributeDrawer(hideIfAttribute, targetObject);
            }

            if (attribute is LabelAttribute labelAttribute)
            {
                return new LabelAttributeDrawer(labelAttribute, targetObject);
            }

            if (attribute is InlineButtonAttribute buttonAttribute)
            {
                return new InlineButtonAttributeDrawer(buttonAttribute, targetObject);
            }

            if (attribute is ValidateAttribute validateAttribute)
            {
                return new ValidationDrawer(validateAttribute, targetObject);
            }

            Debug.LogWarning($"Unsupported attribute: {attribute}");
            return null;
        }
    }
}
