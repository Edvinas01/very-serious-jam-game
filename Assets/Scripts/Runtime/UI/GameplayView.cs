using System;
using InSun.GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayView : View
    {
        [Header("Counters")]
        [SerializeField]
        private TMP_Text remainingTimeText;

        [SerializeField]
        private TMP_Text scoreText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onRemainingTimeChanged;

        private int remainingTimePrev;

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
            set => scoreText.text = value.ToString();
        }
    }
}
