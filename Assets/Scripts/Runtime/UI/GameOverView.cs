using System;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameOverView : View
    {
        [SerializeField]
        private MenuButtonElement restartButton;

        [SerializeField]
        private MenuButtonElement exitButton;

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
