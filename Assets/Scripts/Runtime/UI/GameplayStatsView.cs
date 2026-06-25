using System;
using DoubleD.VerySeriousJamGame.Runtime.Audio;
using InSun.GameCore.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayStatsView : View
    {
        [Header("Text")]
        [SerializeField]
        private TMP_Text remainingTimeText;

        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text speedMultiplierText;

        [Header("Colors")]
        [SerializeField]
        private Color timeRunningOutColor = Color.red;

        [Header("Audio")]
        [SerializeField]
        private AudioSource timeRunningOutSource;

        [SerializeField]
        private AudioData timeRunningOutAudio;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onRemainingTimeChanged;

        [SerializeField]
        private UnityEvent onScoreChanged;

        private Color initialRemainingTimeColor;
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

                    if (IsTimeRunningOut)
                    {
                        PlayTimeRunningOutSfx();
                        remainingTimeText.color = timeRunningOutColor;
                    }
                    else
                    {
                        remainingTimeText.color = initialRemainingTimeColor;
                    }
                }

                if (remainingTimeText)
                {
                    remainingTimeText.text = $"{span.Minutes:00}:{span.Seconds:00}";
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

        public bool IsTimeRunningOut { get; set; }

        protected override void Awake()
        {
            base.Awake();

            initialRemainingTimeColor = remainingTimeText.color;
        }

        private void PlayTimeRunningOutSfx()
        {
            timeRunningOutSource.PlayUsing(timeRunningOutAudio);
        }
    }
}
