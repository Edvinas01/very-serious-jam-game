using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using DoubleD.VerySeriousJamGame.Runtime.Pausing;
using InSun.GameCore;
using InSun.GameCore.Audio;
using InSun.GameCore.Cursors;
using InSun.GameCore.Input;
using InSun.GameCore.Pooling;
using InSun.GameCore.Scenes;
using InSun.GameCore.Transforms;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace DoubleD.VerySeriousJamGame.Runtime
{
    internal sealed class VerySeriousJamGame : Game
    {
        private const string SetupScenePath = "Assets/Scenes/Scene_Setup.unity";

        [Header("Scenes")]
        [SerializeField]
        private SceneData firstScene;

        [SerializeField]
        private SimpleSceneSystem sceneSystem;

        [Header("Core")]
        [SerializeField]
        private InputSystem inputSystem;

        [SerializeField]
        private SimpleObjectPoolingSystem poolingSystem;

        [SerializeField]
        private UnityAudioSystem audioSystem;

        [Header("UI")]
        [SerializeField]
        private CursorSystem cursorSystem;

        [Header("Gameplay")]
        [SerializeField]
        private PauseSystem pauseSystem;

        private static SceneData firstSceneOverrideEditor;

        protected override void OnInitializing()
        {
            // Do not print verbose logs in builds
#if UNITY_EDITOR == false
            if (Debug.isDebugBuild)
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.Full);
                Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.Full);
            }
            else
            {
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
                Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            }
#endif

            // Scenes
            AddObject<ISceneSystem>(sceneSystem);

            // Core
            AddObject(inputSystem);
            AddObject<IObjectPoolingSystem>(poolingSystem);
            AddObject<IAudioSystem>(audioSystem);
            AddObject(new TransformSystem());

            // UI
            AddObject(cursorSystem);

            // Gameplay (aka very game specific)
            AddObject(new PauseSystem());
            AddObject(new GameplaySystem());

            AddListener<PauseStateChangedMessage>(OnPauseStateChanged);
        }

        protected override void OnInitialized()
        {
#if UNITY_WEBGL && UNITY_EDITOR == false
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
#endif

            Application.runInBackground = true;

            inputSystem.EnablePlayerInput();
            inputSystem.EnableUIInput();
        }

        protected override void OnStarted()
        {
            PrimeTweenConfig.warnTweenOnDisabledTarget = false;
            PrimeTweenConfig.SetTweensCapacity(2000);

            StartGameAsync().Forget();
        }

        protected override void OnDisposing()
        {
        }

        protected override void OnDisposed()
        {
        }

        private void OnPauseStateChanged(PauseStateChangedMessage message)
        {
            if (message.IsPausedNext)
            {
                inputSystem.DisablePlayerInput();
            }
            else
            {
                inputSystem.EnablePlayerInput();
            }
        }

        private async UniTaskVoid StartGameAsync()
        {
#if UNITY_EDITOR
            if (firstSceneOverrideEditor)
            {
                await sceneSystem.LoadSceneAsync(new SceneLoadArgs(firstSceneOverrideEditor));
            }
#else
            await sceneSystem.LoadSceneAsync(new SceneLoadArgs(firstScene));
#endif
        }

        // This ensures that every time we hit play from the editor, we go through a similar flow
        // you'd experience in a build:
        // 1. Unity Engine load
        // 2. SnailGame (this class) load
        // 3. Setup scene load
        // 4. Saves load
        // 5. Load starting scene (in editor it's the scene you hit play)
        //
        // ...this ensures that saves and such get loaded before we actually play, otherwise this
        // would result in saves loading after Start() and similar...
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    var sceneDataList = Resources.FindObjectsOfTypeAll<SceneData>();
                    var scene = SceneManager.GetActiveScene();

                    foreach (var sceneData in sceneDataList)
                    {
                        if (scene.path == sceneData.Path)
                        {
                            firstSceneOverrideEditor = sceneData;

                            var setupScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SetupScenePath);
                            EditorSceneManager.playModeStartScene = setupScene;
                            break;
                        }
                    }

                    return;
                }
                case PlayModeStateChange.EnteredEditMode:
                {
                    firstSceneOverrideEditor = null;
                    EditorSceneManager.playModeStartScene = null;
                    break;
                }
            }
        }
#endif
    }
}
