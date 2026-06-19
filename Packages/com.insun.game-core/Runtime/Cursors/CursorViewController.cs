using InSun.GameCore.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InSun.GameCore.Cursors
{
    internal sealed class CursorViewController : ViewController<CursorView>
    {
        private void LateUpdate()
        {
            var pointer = Pointer.current;
            if (pointer == null)
            {
                return;
            }

            View.CursorPosition = pointer.position.ReadValue();
        }

        public void ParentCursorObject(GameObject obj)
        {
            obj.transform.SetParent(View.CursorParent);
            obj.transform.localPosition = Vector3.zero;
        }

        public void SetCursorSprite(Sprite sprite)
        {
            View.CursorSprite = sprite;
        }

        public void ClearCursor()
        {
            View.ClearCursor();
        }

        public void Ping()
        {
            View.Ping();
        }
    }
}
