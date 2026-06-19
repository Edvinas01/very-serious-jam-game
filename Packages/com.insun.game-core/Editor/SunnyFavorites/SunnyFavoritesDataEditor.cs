using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnyFavorites
{
    [CustomEditor(typeof(SunnyFavoritesData))]
    internal sealed class SunnyFavoritesDataEditor : UnityEditor.Editor
    {
        private SerializedProperty itemsProp;
        private ReorderableList favoritesList;

        private GUIStyle windowPaddingStyle;
        private string searchQuery = string.Empty;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            windowPaddingStyle = new GUIStyle { padding = new RectOffset(4, 4, 4, 4) };
            itemsProp = serializedObject.FindProperty("items");

            InitializeFavoritesList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUILayout.VerticalScope(windowPaddingStyle))
            {
                DrawSearchBar();
                EditorGUILayout.Space(2f);

                using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scroll.scrollPosition;

                    if (string.IsNullOrWhiteSpace(searchQuery))
                    {
                        favoritesList.draggable = true;
                        favoritesList.DoLayoutList();
                    }
                    else
                    {
                        DrawFilteredList();
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeFavoritesList()
        {
            favoritesList = new ReorderableList(
                serializedObject: serializedObject,
                elements: itemsProp,
                draggable: true,
                displayHeader: false,
                displayAddButton: true,
                displayRemoveButton: true
            )
            {
                elementHeight = EditorGUIUtility.singleLineHeight + 2f,
                drawElementCallback = DrawElement,
            };
        }

        private void DrawSearchBar()
        {
            searchQuery = EditorGUILayout.TextField(searchQuery, EditorStyles.toolbarSearchField);
        }

        private void DrawFilteredList()
        {
            var removeIndex = -1;
            var indices = GetFilteredIndices();

            foreach (var index in indices)
            {
                var itemProp = itemsProp.GetArrayElementAtIndex(index);

                using (new EditorGUILayout.HorizontalScope())
                {
                    var rect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight);
                    rect.x += 4f;
                    rect.width -= 4f;
                    rect.y += 2f;

                    DrawElement(rect, itemProp);

                    if (GUILayout.Button("-", GUILayout.Width(20f)))
                    {
                        removeIndex = index;
                    }
                }
            }

            if (removeIndex >= 0)
            {
                itemsProp.DeleteArrayElementAtIndex(removeIndex);
            }

            EditorGUILayout.Space(2f);

            if (GUILayout.Button("Add Favorite"))
            {
                itemsProp.InsertArrayElementAtIndex(itemsProp.arraySize);

                var element = itemsProp.GetArrayElementAtIndex(itemsProp.arraySize - 1);
                element.objectReferenceValue = null;
            }
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawElement(rect, itemsProp.GetArrayElementAtIndex(index));
        }

        private static void DrawElement(Rect rect, SerializedProperty itemProp)
        {
            var item = itemProp.objectReferenceValue;
            var isContainsObject = item != false;

            const float buttonWidth = 45f;
            const float buttonSpacing = 2f;

            var buttonCount = isContainsObject ? 2 : 0;
            var totalButtonWidth = buttonCount > 0
                ? buttonCount * buttonWidth + buttonCount * buttonSpacing
                : 0f;

            var fieldRect = new Rect(
                x: rect.x,
                y: rect.y + 1f,
                width: rect.width - totalButtonWidth,
                height: EditorGUIUtility.singleLineHeight
            );

            using (var changeCheck = new EditorGUI.ChangeCheckScope())
            {
                var newItem = EditorGUI.ObjectField(fieldRect, item, typeof(Object), allowSceneObjects: false);
                if (changeCheck.changed)
                {
                    itemProp.objectReferenceValue = newItem;
                }
            }

            if (isContainsObject == false)
            {
                return;
            }

            var buttonX = rect.x + rect.width - totalButtonWidth;
            var buttonRect = new Rect(
                x: buttonX,
                y: rect.y + 1f,
                width: buttonWidth,
                height: EditorGUIUtility.singleLineHeight
            );

            buttonRect.x += buttonSpacing;

            if (GUI.Button(buttonRect, "Select"))
            {
                Selection.activeObject = item;
                EditorGUIUtility.PingObject(item);
            }

            buttonRect.x += buttonWidth + buttonSpacing;

            if (GUI.Button(buttonRect, "Open"))
            {
                AssetDatabase.OpenAsset(item);
            }
        }

        private IReadOnlyList<int> GetFilteredIndices()
        {
            var indices = new List<int>();
            for (var index = 0; index < itemsProp.arraySize; index++)
            {
                var element = itemsProp.GetArrayElementAtIndex(index);
                var item = element.objectReferenceValue;
                if (IsMatchingSearchQuery(item))
                {
                    indices.Add(index);
                }
            }

            return indices;
        }

        private bool IsMatchingSearchQuery(Object item)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return true;
            }

            if (item == false)
            {
                return false;
            }

            return item.name.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
