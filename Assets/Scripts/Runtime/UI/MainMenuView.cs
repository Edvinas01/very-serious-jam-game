using System;
using InSun.GameCore.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class MainMenuView : View
    {
        [SerializeField]
        private MenuButtonElement startButton;

        [SerializeField]
        private MenuButtonElement exitButton;

        [UsedImplicitly]
        public bool IsExitButtonVisible
        {
            set => exitButton.gameObject.SetActive(value);
        }

        public event Action OnStartClicked;

        public event Action OnExitClicked;

        protected override void OnEnable()
        {
            base.OnEnable();

            startButton.OnClicked += OnStartButtonClicked;
            exitButton.OnClicked += OnExitButtonClicked;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            startButton.OnClicked -= OnStartButtonClicked;
            exitButton.OnClicked -= OnExitButtonClicked;
        }

        private void OnStartButtonClicked()
        {
            OnStartClicked?.Invoke();
        }

        private void OnExitButtonClicked()
        {
            OnExitClicked?.Invoke();
        }
    }
}
