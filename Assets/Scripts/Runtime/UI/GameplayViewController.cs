using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore;
using InSun.GameCore.UI;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameplayViewController : ViewController<GameplayView>
    {
        private GameplaySystem gameplaySystem;

        protected override void Awake()
        {
            base.Awake();

            gameplaySystem = Game.GetObject<GameplaySystem>();
        }

        protected override void Update()
        {
            base.Update();

            if (gameplaySystem.State is GameplayState.None or GameplayState.GameOver)
            {
                return;
            }

            View.RemainingTime = gameplaySystem.RemainingTime;
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            Game.AddListener<ScoreChangedMessage>(OnScoreChanged);

            View.RemainingTime = gameplaySystem.RemainingTime;
            View.Score = gameplaySystem.Score;
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            Game.RemoveListener<ScoreChangedMessage>(OnScoreChanged);
        }

        private void OnScoreChanged(ScoreChangedMessage message)
        {
            View.Score = message.ScoreNext;
        }
    }
}
