using System;
using DoubleD.VerySeriousJamGame.Runtime.Audio;
using InSun.GameCore.Animations;
using InSun.GameCore.UI;
using TMPro;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameOverView : View
    {
        [SerializeField]
        private MenuButtonElement restartButton;

        [SerializeField]
        private MenuButtonElement exitButton;

        [Header("Score: Entries")]
        [SerializeField]
        private ScoreElement scoreElementPrefab;

        [SerializeField]
        private Transform scoreElementParent;

        [Header("Score: Total & Creativity")]
        [SerializeField]
        private GameObject scorePanel;

        [SerializeField]
        private TMP_Text totalScoreText;

        [SerializeField]
        private TMP_Text creativityText;

        public bool IsButtonsInteractable
        {
            set
            {
                restartButton.interactable = value;
                exitButton.interactable = value;
            }
        }

        public event Action OnRestartClicked;

        public event Action OnExitClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            restartButton.OnClicked += OnRestartButtonClicked;
            exitButton.OnClicked += OnExitButtonClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            restartButton.OnClicked -= OnRestartButtonClicked;
            exitButton.OnClicked -= OnExitButtonClicked;
        }

        public ScoreElement ShowScoreEntry(
            Sprite icon,
            int paintSessionScore,
            int baseScore,
            float multiplier,
            int totalScore,
            AudioData audioData
        )
        {
            var entry = Instantiate(scoreElementPrefab, scoreElementParent);
            entry.Icon = icon;
            // entry.ScoreText = $"{paintSessionScore} + {baseScore} x{multiplier:F1} = {totalScore}";
            entry.ScoreText = $"{totalScore} art";
            entry.PlaySfx(audioData);

            if (entry.TryGetComponent<TweenAnimation>(out var tween))
            {
                tween.Play();
            }

            return entry;
        }

        public void ShowTotalScore(int score, float creativity)
        {
            totalScoreText.text = score.ToString();
            creativityText.text = $"x{creativity:F2}";
            scorePanel.SetActive(true);

            if (scorePanel.TryGetComponent<TweenAnimation>(out var tween))
            {
                tween.Play();
            }
        }

        public void HideTotalScore()
        {
            scorePanel.SetActive(false);
        }

        private void OnRestartButtonClicked()
        {
            OnRestartClicked?.Invoke();
        }

        private void OnExitButtonClicked()
        {
            OnExitClicked?.Invoke();
        }
    }
}
