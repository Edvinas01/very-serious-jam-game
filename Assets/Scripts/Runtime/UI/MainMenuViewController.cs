using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.UI;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class MainMenuViewController : ViewController<MainMenuView>
    {
        [SerializeField]
        private SceneData gameplayScene;

        private ISceneSystem sceneSystem;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = Game.GetObject<ISceneSystem>();
        }

        protected override void Start()
        {
            base.Start();

#if UNITY_WEBGL
            View.IsExitButtonVisible = false;
#endif
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            View.OnStartClicked += OnViewStartClicked;
            View.OnExitClicked += OnViewExitClicked;
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            View.OnStartClicked -= OnViewStartClicked;
            View.OnExitClicked -= OnViewExitClicked;
        }

        private void OnViewStartClicked()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(gameplayScene));
        }

        private static void OnViewExitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
