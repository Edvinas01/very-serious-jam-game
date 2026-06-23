using System.Threading;
using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameOverViewController : ViewController<GameOverView>
    {
        [Header("Scenes")]
        [SerializeField]
        private SceneData gameplayScene;

        [SerializeField]
        private SceneData mainMenuScene;

        [Header("Timings")]
        [Min(0f)]
        [SerializeField]
        private float entrySpawnDelay = 0.5f;

        private GameplaySystem gameplaySystem;
        private ISceneSystem sceneSystem;

        protected override void Awake()
        {
            base.Awake();

            gameplaySystem = Game.GetObject<GameplaySystem>();
            sceneSystem = Game.GetObject<ISceneSystem>();
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            View.OnRestartClicked += OnViewRestartClicked;
            View.OnExitClicked += OnViewExitClicked;

            SpawnScoreEntriesAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            View.OnRestartClicked -= OnViewRestartClicked;
            View.OnExitClicked -= OnViewExitClicked;
        }

        private void OnViewRestartClicked()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(gameplayScene));
        }

        private void OnViewExitClicked()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(mainMenuScene));
        }

        private async UniTaskVoid SpawnScoreEntriesAsync(CancellationToken cancellationToken)
        {
            View.IsButtonsInteractable = false;
            View.HideTotalScore();

            foreach (var entry in gameplaySystem.ScoreEntries)
            {
                await UniTask.WaitForSeconds(entrySpawnDelay, cancellationToken: cancellationToken);
                View.ShowScoreEntry(
                    icon: entry.Data.Icon,
                    paintSessionScore: entry.PaintableScore,
                    baseScore: entry.BaseScore,
                    multiplier: entry.ScoreMultiplier,
                    totalScore: entry.ScoreResult,
                    audioData: entry.Data.SpeakAudio
                );
            }

            await UniTask.WaitForSeconds(entrySpawnDelay, cancellationToken: cancellationToken);

            View.IsButtonsInteractable = true;
            View.ShowTotalScore(gameplaySystem.Score);
        }
    }
}
