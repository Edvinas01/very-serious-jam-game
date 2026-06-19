using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace InSun.GameCore.Editor.SunnySettings
{
    internal sealed class SettingsTreeView : TreeView<int>
    {
        private readonly List<AssetTreeViewItem> selectedAssetItems = new();

        public event Action<IEnumerable<AssetTreeViewItem>> OnSelectionChanged;

        public SettingsTreeView(TreeViewState<int> state) : base(state)
        {
            enableItemHovering = true;
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            useScrollView = true;
        }

        public IEnumerable<AssetTreeViewItem> SelectedAssetItems => selectedAssetItems;

        protected override TreeViewItem<int> BuildRoot()
        {
            var root = new TreeViewItem<int>(id: "Root".GetHashCode(), depth: -1, displayName: "Root");
            var items = CreateItems();

            if (items.Count == 0)
            {
                var errorItem = new TreeViewItem<int>(
                    id: "Error".GetHashCode(),
                    depth: 0,
                    displayName: "No settings found..."
                );

                root.AddChild(errorItem);
            }
            else
            {
                foreach (var item in items)
                {
                    root.AddChild(item);
                }
            }

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            selectedAssetItems.Clear();

            foreach (var selectedId in selectedIds)
            {
                var item = FindItem(selectedId, rootItem);
                if (item is AssetTreeViewItem assetItem)
                {
                    selectedAssetItems.Add(assetItem);
                }
            }

            OnSelectionChanged?.Invoke(selectedAssetItems);
        }

        protected override void SingleClickedItem(int id)
        {
            var item = FindItem(id, rootItem);
            if (item is CatalogTreeViewItem == false)
            {
                return;
            }

            var isExpandedNext = IsExpanded(id) == false;
            SetExpanded(id, isExpandedNext);
        }

        protected override bool CanMultiSelect(TreeViewItem<int> item)
        {
            if (item is not AssetTreeViewItem)
            {
                return false;
            }

            return true;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = args.item;
            var rowRect = args.rowRect;
            rowRect.xMin += GetContentIndent(item);

            switch (item)
            {
                case CatalogTreeViewItem folderItem:
                {
                    EditorGUI.LabelField(rowRect, new GUIContent(item.displayName, folderItem.Icon));
                    break;
                }
                case AssetTreeViewItem assetItem:
                {
                    EditorGUI.LabelField(rowRect, new GUIContent(item.displayName, assetItem.Icon));
                    break;
                }
                default:
                {
                    base.RowGUI(args);
                    break;
                }
            }
        }

        private static List<TreeViewItem<int>> CreateItems()
        {
            var entries = CollectSettingsEntries();
            var folderCache = new Dictionary<string, CatalogTreeViewItem>();
            var items = new List<TreeViewItem<int>>();

            foreach (var entry in entries)
            {
                var menuPath = entry.MenuPath;
                var menuPathSegments = menuPath.Split('/');
                var depth = menuPathSegments.Length - 1;

                var assetItem = new AssetTreeViewItem(
                    id: entry.AssetGuid.GetHashCode(),
                    depth: depth,
                    displayName: entry.MenuName,
                    scriptableObject: entry.ScriptableObject,
                    attribute: entry.Attribute
                );

                var folder = GetOrCreateFolder(menuPath);
                folder.AddChild(assetItem);
            }

            return items;

            CatalogTreeViewItem GetOrCreateFolder(string menuPath)
            {
                if (folderCache.TryGetValue(menuPath, out var existingFolder))
                {
                    return existingFolder;
                }

                var segments = menuPath.Split('/');
                var depth = segments.Length - 1;

                var folder = new CatalogTreeViewItem(
                    id: menuPath.GetHashCode(),
                    depth: depth,
                    displayName: segments.Last()
                );

                folderCache[menuPath] = folder;

                if (segments.Length > 1)
                {
                    var parentPath = string.Join('/', segments.Take(segments.Length - 1));
                    GetOrCreateFolder(parentPath).AddChild(folder);
                }
                else
                {
                    items.Add(folder);
                }

                return folder;
            }
        }

        private static IReadOnlyList<SunnySettingsEntry> CollectSettingsEntries()
        {
            var entries = new List<SunnySettingsEntry>();
            var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");
            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

                var scriptableObjectType = scriptableObject.GetType();
                var attribute = scriptableObjectType.GetCustomAttribute<SunnySettingsAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                var entry = new SunnySettingsEntry(
                    assetPath: assetPath,
                    assetGuid: guid,
                    menuPath: GetMenuPath(attribute),
                    menuName: GetAssetName(scriptableObject, attribute),
                    scriptableObject: scriptableObject,
                    attribute: attribute
                );
                entries.Add(entry);
            }

            return entries
                .OrderBy(entry => entry.AssetPath)
                .ThenBy(entry => entry.MenuName)
                .ToList();

            static string GetMenuPath(SunnySettingsAttribute attribute)
            {
                return attribute.MenuPath.TrimStart('/').TrimEnd('/');
            }

            static string GetAssetName(ScriptableObject obj, SunnySettingsAttribute attribute)
            {
                var nameGetter = attribute.NameGetter;
                if (ReflectionUtilities.TryGetMember(obj, nameGetter, out var member) == false)
                {
                    return obj.name;
                }

                if (ReflectionUtilities.TryGetValue<string>(obj, member, out var value) == false)
                {
                    return obj.name;
                }

                return value;
            }
        }
    }

    internal readonly struct SunnySettingsEntry
    {
        public string AssetPath { get; }

        public string AssetGuid { get; }

        public string MenuPath { get; }

        public string MenuName { get; }

        public ScriptableObject ScriptableObject { get; }

        public SunnySettingsAttribute Attribute { get; }

        public SunnySettingsEntry(
            string assetPath,
            string assetGuid,
            string menuPath,
            string menuName,
            ScriptableObject scriptableObject,
            SunnySettingsAttribute attribute
        )
        {
            AssetPath = assetPath;
            AssetGuid = assetGuid;
            MenuPath = menuPath;
            MenuName = menuName;
            ScriptableObject = scriptableObject;
            Attribute = attribute;
        }
    }

    internal sealed class CatalogTreeViewItem : TreeViewItem<int>
    {
        public Texture Icon => SunnySettingsIcons.FolderIcon;

        public CatalogTreeViewItem(int id, int depth, string displayName) : base(id, depth, displayName)
        {
        }
    }

    internal sealed class AssetTreeViewItem : TreeViewItem<int>
    {
        public ScriptableObject ScriptableObject { get; }

        public SunnySettingsAttribute Attribute { get; }

        public Texture Icon
        {
            get
            {
                var scriptableObject = ScriptableObject;
                if (scriptableObject == false)
                {
                    return SunnySettingsIcons.ScriptableObjectIcon;
                }

                var objectIcon = EditorGUIUtility.GetIconForObject(scriptableObject);
                if (objectIcon == false)
                {
                    return SunnySettingsIcons.ScriptableObjectIcon;
                }

                return objectIcon;
            }
        }

        public AssetTreeViewItem(
            int id,
            int depth,
            string displayName,
            ScriptableObject scriptableObject,
            SunnySettingsAttribute attribute
        ) : base(id, depth, displayName)
        {
            ScriptableObject = scriptableObject;
            Attribute = attribute;
        }
    }
}
