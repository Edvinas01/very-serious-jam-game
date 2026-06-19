using InSun.GameCore.Animations;
using InSun.GameCore.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace InSun.GameCore.Cursors
{
    internal sealed class CursorView : View
    {
        [Header("Cursor")]
        [SerializeField]
        private RectTransform cursorParent;

        [FormerlySerializedAs("pointerImage")]
        [SerializeField]
        private Image cursorImage;

        [FormerlySerializedAs("pointerOffset")]
        [SerializeField]
        private Vector2 cursorOffset = new(16f, 8f);

        [Header("Juice")]
        [SerializeField]
        private TweenAnimation pingTween;

        public RectTransform CursorParent => cursorParent;

        public Sprite CursorSprite
        {
            set
            {
                if (value)
                {
                    cursorImage.enabled = true;
                    cursorImage.sprite = value;
                }
                else
                {
                    cursorImage.enabled = false;
                    cursorImage.sprite = null;
                }
            }
        }

        public Vector2 CursorPosition
        {
            set
            {
                cursorParent.anchorMin = Vector2.zero;
                cursorParent.anchorMax = Vector2.zero;
                cursorParent.pivot = new Vector2(0f, 1f);
                cursorParent.anchoredPosition = value + cursorOffset;
            }
        }

        public void ClearCursor()
        {
            cursorImage.enabled = false;
            cursorImage.sprite = null;
        }

        public void Ping()
        {
            if (pingTween)
            {
                pingTween.Play();
            }
        }
    }
}
