using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayStatsViewController : ViewController<GameplayStatsView>
    {
        [Header("Timings")]
        [SerializeField]
        private int timeRunningOutSeconds = 5;

        private GameplaySystem gameplaySystem;

        protected override void Awake()
        {
            base.Awake();

            gameplaySystem = Game.GetObject<GameplaySystem>();
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            Game.AddListener<ScoreChangedMessage>(OnScoreChanged);

            View.IsTimeRunningOut = gameplaySystem.RemainingTime <= timeRunningOutSeconds;
            View.RemainingTime = gameplaySystem.RemainingTime;
            View.Score = gameplaySystem.Score;
            View.SpeedMultiplier = gameplaySystem.CurrentMultiplier;
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            Game.RemoveListener<ScoreChangedMessage>(OnScoreChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Game.RemoveListener<ScoreChangedMessage>(OnScoreChanged);
        }

        protected override void Update()
        {
            base.Update();

            if (gameplaySystem.State is GameplayState.None)
            {
                return;
            }

            View.IsTimeRunningOut = gameplaySystem.RemainingTime <= timeRunningOutSeconds;
            View.RemainingTime = gameplaySystem.RemainingTime;
            View.SpeedMultiplier = gameplaySystem.CurrentMultiplier;
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            View.Score = message.ScoreNext;
        }
    }
}
