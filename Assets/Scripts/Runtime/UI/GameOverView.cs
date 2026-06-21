using System;
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

        [Header("Score: Total")]
        [SerializeField]
        private GameObject scorePanel;

        [SerializeField]
        private TMP_Text totalScoreText;

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

        public ScoreElement ShowScoreEntry(Sprite icon, int scorePerObject, int count)
        {
            var entry = Instantiate(scoreElementPrefab, scoreElementParent);
            entry.Icon = icon;
            entry.ScoreText = $"{scorePerObject} x {count} = {scorePerObject * count}";

            if (entry.TryGetComponent<TweenAnimation>(out var tween))
            {
                tween.Play();
            }

            return entry;
        }

        public void ShowTotalScore(int score)
        {
            totalScoreText.text = score.ToString();
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
