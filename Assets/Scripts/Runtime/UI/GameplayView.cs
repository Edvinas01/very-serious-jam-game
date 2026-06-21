using System;
using InSun.GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayView : View
    {
        [Header("Text")]
        [SerializeField]
        private TMP_Text paintableNameText;

        [SerializeField]
        private TMP_Text remainingTimeText;

        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text paintPercentageText;

        [SerializeField]
        private TMP_Text speedMultiplierText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onRemainingTimeChanged;

        [SerializeField]
        private UnityEvent onScoreChanged;

        [SerializeField]
        private UnityEvent onPaintAmountChanged;

        private int remainingTimePrev;
        private int scorePrev;
        private int paintAmountPrev;

        public float RemainingTime
        {
            set
            {
                var span = TimeSpan.FromSeconds(value);

                var remainingTimeNext = (int)span.TotalSeconds;
                if (remainingTimeNext != remainingTimePrev)
                {
                    onRemainingTimeChanged.Invoke();
                }

                remainingTimeText.text = $"{span.Minutes}:{span.Seconds:00}";
                remainingTimePrev = remainingTimeNext;
            }
        }

        public int Score
        {
            set
            {
                var scoreNext = value;
                if (scorePrev != scoreNext)
                {
                    onScoreChanged.Invoke();
                }

                scoreText.text = value.ToString();
                scorePrev = scoreNext;
            }
        }

        public float PaintAmount
        {
            set
            {
                var paintPercentageNext = Mathf.RoundToInt(value * 100f);
                if (paintPercentageNext != paintAmountPrev)
                {
                    onPaintAmountChanged.Invoke();
                }

                paintPercentageText.text = $"{paintPercentageNext}%";
                paintAmountPrev = paintPercentageNext;
            }
        }

        public float SpeedMultiplier
        {
            set => speedMultiplierText.text = $"x{value:F1}";
        }

        public string PaintableName
        {
            set => paintableNameText.text = value;
        }
    }
}
