using System;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class PauseMenuView : View
    {
        [SerializeField]
        private MenuButtonElement resumeButton;

        [SerializeField]
        private MenuButtonElement restartButton;

        [SerializeField]
        private MenuButtonElement exitButton;

        public event Action OnResumeClicked;
        public event Action OnRestartClicked;
        public event Action OnExitClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            resumeButton.OnClicked += OnResumeButtonClicked;
            restartButton.OnClicked += OnRestartButtonClicked;
            exitButton.OnClicked += OnExitButtonClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            resumeButton.OnClicked -= OnResumeButtonClicked;
            restartButton.OnClicked -= OnRestartButtonClicked;
            exitButton.OnClicked -= OnExitButtonClicked;
        }

        private void OnResumeButtonClicked()
        {
            OnResumeClicked?.Invoke();
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
