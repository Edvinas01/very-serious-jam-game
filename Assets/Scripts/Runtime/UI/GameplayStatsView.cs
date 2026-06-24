using System;
using InSun.GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayStatsView : View
    {
        [SerializeField]
        private TMP_Text remainingTimeText;

        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text speedMultiplierText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onRemainingTimeChanged;

        [SerializeField]
        private UnityEvent onScoreChanged;

        private int remainingTimePrev;
        private int scorePrev;

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

                if (remainingTimeText)
                {
                    remainingTimeText.text = $"{span.Minutes}:{span.Seconds:00}";
                }

                remainingTimePrev = remainingTimeNext;
            }
        }

        public int Score
        {
            set
            {
                if (scorePrev != value)
                {
                    onScoreChanged.Invoke();
                }

                if (scoreText)
                {
                    scoreText.text = value.ToString();
                }

                scorePrev = value;
            }
        }

        public float SpeedMultiplier
        {
            set
            {
                if (speedMultiplierText)
                {
                    speedMultiplierText.text = $"x{value:F1}";
                }
            }
        }
    }
}
