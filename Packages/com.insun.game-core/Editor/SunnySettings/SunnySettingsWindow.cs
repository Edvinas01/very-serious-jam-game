using System.Collections.Generic;
using System.Linq;
using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnySettings
{
    internal sealed class SunnySettingsWindow : EditorWindow
    {
        [SerializeField]
        private TreeViewState<int> treeViewState;

        private const float DefaultLeftPanelWidth = 280f;
        private const float MinLeftPanelWidth = 120f;
        private const float SplitterWidth = 4f;

        private static GUIStyle cachedItemContentStyle;

        private Vector2 contentScrollPosition;

        private SettingsTreeView treeView;
        private UnityEditor.Editor cachedEditor;

        private float menuPanelWidth = DefaultLeftPanelWidth;
        private bool isDraggingSplitter;

        [MenuItem(
            MenuConstants.BaseWindowMenuName + "/Sunny Settings",
            priority = MenuConstants.BaseWindowMenuPriority
        )]
        private static void ShowWindow()
        {
            var window = GetWindow<SunnySettingsWindow>();
            window.titleContent = new GUIContent("Sunny Settings", SunnySettingsIcons.SettingsIcon);

            var minSize = window.minSize;
            minSize.x = 200f;
            minSize.y = 300f;
            window.minSize = minSize;

            window.Show();
        }

        private void OnEnable()
        {
            treeViewState ??= new TreeViewState<int>();
            treeView = new SettingsTreeView(treeViewState);
            treeView.OnSelectionChanged += OnTreeSelectionChanged;
            treeView.Reload();
        }

        private void OnDisable()
        {
            if (treeView != null)
            {
                treeView.OnSelectionChanged -= OnTreeSelectionChanged;
            }

            DestroyImmediate(cachedEditor);
        }

        private void OnGUI()
        {
            DrawToolbar();

            var splitterRect = new Rect(
                menuPanelWidth,
                EditorGUIUtility.singleLineHeight + 2f,
                SplitterWidth,
                position.height - EditorGUIUtility.singleLineHeight - 2f
            );

            DrawMenuTreePanel(splitterRect);
            DrawSplitter(splitterRect);
            DrawContentPanel(splitterRect);
        }

        public void ReloadTree()
        {
            treeView.Reload();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60f)))
                {
                    treeView.Reload();
                    ClearInspector();
                }

                if (GUILayout.Button("Expand All", EditorStyles.toolbarButton, GUILayout.Width(75f)))
                {
                    treeView.ExpandAll();
                }

                if (GUILayout.Button("Collapse All", EditorStyles.toolbarButton, GUILayout.Width(75f)))
                {
                    treeView.CollapseAll();
                }

                GUILayout.FlexibleSpace();
            }
        }

        private void DrawMenuTreePanel(Rect splitterRect)
        {
            var leftRect = new Rect(0f, splitterRect.y, menuPanelWidth, splitterRect.height);
            treeView.OnGUI(leftRect);
        }

        private void DrawSplitter(Rect splitterRect)
        {
            EditorGUI.DrawRect(splitterRect, new Color(0f, 0f, 0f, 0.3f));
            EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);

            var evt = Event.current;

            switch (evt.type)
            {
                case EventType.MouseDown when splitterRect.Contains(evt.mousePosition):
                {
                    isDraggingSplitter = true;
                    evt.Use();
                    break;
                }
                case EventType.MouseDrag when isDraggingSplitter:
                {
                    menuPanelWidth = Mathf.Clamp(
                        evt.mousePosition.x,
                        MinLeftPanelWidth,
                        position.width - MinLeftPanelWidth - SplitterWidth
                    );

                    Repaint();
                    evt.Use();
                    break;
                }
                case EventType.MouseUp when isDraggingSplitter:
                {
                    isDraggingSplitter = false;
                    evt.Use();
                    break;
                }
            }
        }

        private void DrawContentPanel(Rect splitterRect)
        {
            var rightX = menuPanelWidth + SplitterWidth;
            var rightRect = new Rect(rightX, splitterRect.y, position.width - rightX, splitterRect.height);
            GUILayout.BeginArea(rightRect);

            var selectedItems = treeView.SelectedAssetItems.ToList();
            if (selectedItems.Count <= 0)
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(
                        "Select a Scriptable Object from the tree",
                        EditorStyles.centeredGreyMiniLabel
                    );

                    GUILayout.FlexibleSpace();
                }

                GUILayout.FlexibleSpace();
            }
            else
            {
                using (var scope = new EditorGUILayout.ScrollViewScope(contentScrollPosition))
                {
                    DrawInspector(selectedItems);
                    contentScrollPosition = scope.scrollPosition;
                }
            }

            GUILayout.EndArea();
        }

        private void DrawInspector(List<AssetTreeViewItem> items)
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                var label = items.Count == 1
                    ? items[0].displayName
                    : $"{items.Count} Scriptable Objects selected";

                GUILayout.Label(label, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();

                if (items.Count == 1 && GUILayout.Button("Ping", EditorStyles.toolbarButton, GUILayout.Width(40f)))
                {
                    var firstItem = items[0];
                    EditorGUIUtility.PingObject(firstItem.ScriptableObject);
                }

                if (GUILayout.Button("Select", EditorStyles.toolbarButton, GUILayout.Width(50f)))
                {
                    Selection.objects = items
                        .Select(item => item.ScriptableObject)
                        .Where(asset => asset)
                        .Cast<Object>()
                        .ToArray();
                }
            }

            var assets = items
                .Select(item => item.ScriptableObject)
                .Where(asset => asset)
                .Cast<Object>()
                .ToArray();

            if (assets.Length == 0)
            {
                EditorGUILayout.HelpBox("No Scriptable Objects selected", MessageType.Warning);
                return;
            }

            var firstType = assets[0].GetType();
            foreach (var asset in assets)
            {
                if (firstType != asset.GetType())
                {
                    var typeStr = "\n• " + string.Join("\n• ", assets.Select(a => a.GetType().Name).Distinct());
                    EditorGUILayout.HelpBox($"Selection contains Scriptable Objects of different types: {typeStr}", MessageType.Warning);
                    return;
                }
            }

            UnityEditor.Editor.CreateCachedEditorWithContext(
                assets,
                context: null,
                editorType: null,
                previousEditor: ref cachedEditor
            );

            using var changeScope = new EditorGUI.ChangeCheckScope();
            cachedEditor.OnInspectorGUI();

            if (changeScope.changed)
            {
                foreach (var asset in assets)
                {
                    EditorUtility.SetDirty(asset);
                }
            }
        }

        private void OnTreeSelectionChanged(IEnumerable<AssetTreeViewItem> items)
        {
            contentScrollPosition = Vector2.zero;
            DestroyImmediate(cachedEditor);
            Repaint();
        }

        private void ClearInspector()
        {
            contentScrollPosition = Vector2.zero;
            DestroyImmediate(cachedEditor);
        }
    }
}
