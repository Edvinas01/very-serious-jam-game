using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Cursors
{
    public sealed class CursorSystem : MonoBehaviour
    {
        [SerializeField]
        private CursorViewController cursorViewController;

        [SerializeField]
        private Sprite defaultCursor;

        [SerializeField]
        private bool isHideSystemCursor;

        private readonly List<ICursorStackEntry> stack = new();
        private uint nextId;

        private void Start()
        {
            if (isHideSystemCursor)
            {
                Cursor.visible = false;
            }

            if (defaultCursor)
            {
                SetCursor(defaultCursor);
            }
        }

        public ICursor PushCursor(Sprite sprite)
        {
            var entryId = nextId++;
            var entry = new SpriteCursorStackEntry(id: entryId, sprite: sprite);

            if (stack.Count > 0)
            {
                var top = stack[stack.Count - 1];
                top.Hide();
            }

            stack.Add(entry);
            entry.Show(cursorViewController);

            return new CursorHandle(this, entryId);
        }

        public ICursor PushCursor(GameObject obj)
        {
            var entryId = nextId++;
            var entry = new GameObjectCursorStackEntry(id: entryId, gameObject: obj);

            if (stack.Count > 0)
            {
                var top = stack[stack.Count - 1];
                top.Hide();
            }

            stack.Add(entry);
            entry.Show(cursorViewController);

            return new CursorHandle(this, entryId);
        }

        public void ApplyDefaultCursor()
        {
            SetCursor(defaultCursor);
        }

        public void ClearCursor()
        {
            cursorViewController.ClearCursor();
        }

        public void SetCursor(Sprite cursorSprite)
        {
            cursorViewController.SetCursorSprite(cursorSprite);
        }

        public void PingCursor()
        {
            cursorViewController.Ping();
        }

        private void PopCursor(uint id)
        {
            var removedIndex = -1;
            for (var index = stack.Count - 1; index >= 0; index--)
            {
                var entry = stack[index];
                if (entry.Id != id)
                {
                    continue;
                }

                removedIndex = index;
                break;
            }

            if (removedIndex < 0)
            {
                return;
            }

            var isRemovedTop = removedIndex == stack.Count - 1;
            var removedEntry = stack[removedIndex];

            stack.RemoveAt(removedIndex);

            removedEntry.Remove();

            if (isRemovedTop == false)
            {
                return;
            }

            if (stack.Count > 0)
            {
                var top = stack[stack.Count - 1];
                top.Show(cursorViewController);
            }
            else
            {
                cursorViewController.SetCursorSprite(defaultCursor);
            }
        }

        private sealed class CursorHandle : ICursor
        {
            private readonly CursorSystem system;
            private readonly uint id;

            public CursorHandle(CursorSystem system, uint id)
            {
                this.system = system;
                this.id = id;
            }

            public void Dispose()
            {
                if (Game.IsQuitting)
                {
                    return;
                }

                system.PopCursor(id);
            }
        }

        private sealed class SpriteCursorStackEntry : ICursorStackEntry
        {
            private readonly Sprite sprite;

            public uint Id { get; }

            public SpriteCursorStackEntry(uint id, Sprite sprite)
            {
                Id = id;
                this.sprite = sprite;
            }

            public void Show(CursorViewController controller)
            {
                controller.SetCursorSprite(sprite);
            }

            public void Hide()
            {
            }

            public void Remove()
            {
            }
        }

        private sealed class GameObjectCursorStackEntry : ICursorStackEntry
        {
            private readonly GameObject gameObject;
            private bool isInitialized;

            public uint Id { get; }

            public GameObjectCursorStackEntry(uint id, GameObject gameObject)
            {
                Id = id;
                this.gameObject = gameObject;
            }

            public void Show(CursorViewController controller)
            {
                controller.ClearCursor();

                if (isInitialized == false)
                {
                    controller.ParentCursorObject(gameObject);
                    isInitialized = true;
                }

                gameObject.SetActive(true);
            }

            public void Hide()
            {
                gameObject.SetActive(false);
            }

            public void Remove()
            {
                Destroy(gameObject);
            }
        }

        private interface ICursorStackEntry
        {
            public uint Id { get; }

            public void Show(CursorViewController controller);

            public void Hide();

            public void Remove();
        }
    }
}
