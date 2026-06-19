using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class GameOverViewController : ViewController<GameOverView>
    {
        [SerializeField]
        private SceneData gameplayScene;

        [SerializeField]
        private SceneData mainMenuScene;

        private ISceneSystem sceneSystem;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = Game.GetObject<ISceneSystem>();
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            View.OnRestartClicked += OnViewRestartClicked;
            View.OnExitClicked += OnViewExitClicked;
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
    }
}
