using InSun.GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayPaintableView : View
    {
        [Header("Paintable")]
        [SerializeField]
        private Image paintableIconImage;

        [SerializeField]
        private TMP_Text paintableNameText;

        [SerializeField]
        private Image paintPercentageImage;

        [SerializeField]
        private TMP_Text paintPercentageText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPaintAmountChanged;

        private int paintAmountPrev;

        public float PaintAmount
        {
            set
            {
                var paintPercentageNext = Mathf.RoundToInt(value * 100f);
                if (paintPercentageNext != paintAmountPrev)
                {
                    onPaintAmountChanged.Invoke();
                }

                if (paintPercentageImage)
                {
                    paintPercentageImage.fillAmount = value;
                }

                if (paintPercentageText)
                {
                    paintPercentageText.text = $"{paintPercentageNext}%";
                }

                paintAmountPrev = paintPercentageNext;
            }
        }

        public string PaintableName
        {
            set => paintableNameText.text = value;
        }

        public Sprite PaintableIcon
        {
            set
            {
                paintableIconImage.enabled = value == true;
                paintableIconImage.sprite = value;
            }
        }
    }
}
