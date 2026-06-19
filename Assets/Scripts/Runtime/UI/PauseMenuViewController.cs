using DoubleD.VerySeriousJamGame.Runtime.Pausing;
using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class PauseMenuViewController : ViewController<PauseMenuView>
    {
        [SerializeField]
        private SceneData gameplayScene;

        [SerializeField]
        private SceneData mainMenuScene;

        [SerializeField]
        private InputActionReference pauseInputAction;

        private ISceneSystem sceneSystem;
        private PauseSystem pauseSystem;

        protected override void Awake()
        {
            base.Awake();

            sceneSystem = Game.GetObject<ISceneSystem>();
            pauseSystem = Game.GetObject<PauseSystem>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            pauseInputAction.action.performed += OnPausePerformed;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            pauseInputAction.action.performed -= OnPausePerformed;

            pauseSystem.UnPause(this);
        }

        protected override void OnViewShowEntered()
        {
            base.OnViewShowEntered();

            pauseSystem.Pause(this);

            View.OnResumeClicked += OnViewResumeClicked;
            View.OnRestartClicked += OnViewRestartClicked;
            View.OnExitClicked += OnViewExitClicked;
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            pauseSystem.UnPause(this);

            View.OnResumeClicked -= OnViewResumeClicked;
            View.OnRestartClicked -= OnViewRestartClicked;
            View.OnExitClicked -= OnViewExitClicked;
        }

        private void OnPausePerformed(InputAction.CallbackContext ctx)
        {
            ToggleVisibility();
        }

        private void OnViewResumeClicked()
        {
            HideView();
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
