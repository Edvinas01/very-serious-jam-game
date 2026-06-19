using System.Collections.Generic;
using InSun.GameCore;
using InSun.GameCore.Objects;
using InSun.GameCore.Scenes;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Pausing
{
    internal sealed class PauseSystem : ILifecycleListener
    {
        private ISceneSystem sceneSystem;

        private readonly List<object> pausedByList = new();
        private float initialTimeScale;

        public void OnInitialized()
        {
            initialTimeScale = Time.timeScale;
            sceneSystem = Game.GetObject<ISceneSystem>();

            Game.AddListener<SceneLoadStartedMessage>(OnSceneLoadStarted);
        }

        public void OnDisposed()
        {
            Game.RemoveListener<SceneLoadStartedMessage>(OnSceneLoadStarted);
        }

        public void Pause(object pausedBy)
        {
            if (sceneSystem.IsSceneLoading)
            {
                Debug.LogWarning("Cannot pause, a scene is being loaded");
                return;
            }

            if (pausedByList.Contains(pausedBy))
            {
                return;
            }

            pausedByList.Add(pausedBy);

            if (pausedByList.Count == 1)
            {
                Time.timeScale = 0f;
                Game.PublishMessage(new PauseStateChangedMessage(isPausedPrev: false, isPausedNext: true));
            }
        }

        public void UnPause(object pausedBy)
        {
            if (pausedByList.Count <= 0)
            {
                return;
            }

            pausedByList.Remove(pausedBy);

            if (pausedByList.Count > 0)
            {
                return;
            }

            Time.timeScale = initialTimeScale;
            Game.PublishMessage(new PauseStateChangedMessage(isPausedPrev: true, isPausedNext: false));
        }

        public void UnPause()
        {
            if (pausedByList.Count <= 0)
            {
                return;
            }

            pausedByList.Clear();

            Time.timeScale = initialTimeScale;
            Game.PublishMessage(new PauseStateChangedMessage(isPausedPrev: true, isPausedNext: false));
        }

        private void OnSceneLoadStarted(SceneLoadStartedMessage message)
        {
            UnPause();
        }
    }
}
