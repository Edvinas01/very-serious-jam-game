using InSun.GameCore.Messaging;
using InSun.GameCore.Objects;
using UnityEngine;

namespace InSun.GameCore
{
    public abstract partial class Game : MonoBehaviour
    {
        private static bool isWarnedNullInstance;
        private static Game instance;

        private IMessageBus messageBus;
        private IObjectRegistry objectRegistry;

        public static bool IsQuitting { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGame()
        {
            var gameResource = Resources.Load<Game>("Game");
            if (gameResource == false)
            {
                Debug.LogError(
                    ""
                    + $"Cannot initialize Core, make sure you've created a "
                    + $"Resources/Game.prefab and added a script which inherits Game.cs"
                );

                return;
            }

            IsQuitting = false;
            Application.quitting -= OnApplicationQuitting;
            Application.quitting += OnApplicationQuitting;

            gameResource.gameObject.SetActive(false);
            try
            {
                var gameInstance = Instantiate(gameResource);
                gameInstance.name = gameResource.GetType().Name;

                DontDestroyOnLoad(gameInstance);
                instance = gameInstance;

                var objectRegistry = instance.CreateObjectRegistry();
                var messageBus = instance.CreateMessageBus();

                instance.objectRegistry = objectRegistry;
                instance.messageBus = messageBus;

                objectRegistry.IsInitialized = false;
                instance.OnInitializing();
                gameInstance.gameObject.SetActive(true);

                objectRegistry.Initialize();
                objectRegistry.IsInitialized = true;
                objectRegistry.OnObjectRegistered += OnObjectRegistered;
                objectRegistry.OnObjectUnregistered += OnObjectUnregistered;

                instance.OnInitialized();
            }
            finally
            {
                gameResource.gameObject.SetActive(true);
            }
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeEditorHooks()
        {
            UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
            {
                IsQuitting = false;
            }

            if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
            {
                instance = null;
                isWarnedNullInstance = false;
            }
        }
#endif

        protected void Start()
        {
            OnStarted();
        }

        protected void FixedUpdate()
        {
            objectRegistry.FixedUpdate();
        }

        protected void Update()
        {
            objectRegistry.Update();
        }

        protected void LateUpdate()
        {
            objectRegistry.LateUpdate();
        }

        protected void OnDestroy()
        {
            OnDisposing();
            objectRegistry.Dispose();
            OnDisposed();
        }

        protected abstract void OnInitializing();

        protected abstract void OnInitialized();

        protected abstract void OnStarted();

        protected abstract void OnDisposing();

        protected abstract void OnDisposed();

        protected virtual IObjectRegistry CreateObjectRegistry()
        {
            return new SimpleObjectRegistry();
        }

        protected virtual IMessageBus CreateMessageBus()
        {
            return new SimpleMessageBus();
        }

        private static void OnApplicationQuitting()
        {
            PublishMessage(new ApplicationQuittingMessage());
            IsQuitting = true;
        }

        private static void OnObjectRegistered(object obj)
        {
            PublishMessage(new ObjectRegisteredMessage(obj));
        }

        private static void OnObjectUnregistered(object obj)
        {
            PublishMessage(new ObjectUnregisteredMessage(obj));
        }

        private static bool IsInstanceReadyEditor()
        {
#if UNITY_EDITOR
            if (IsQuitting)
            {
                return false;
            }

            if (instance)
            {
                return true;
            }

            if (isWarnedNullInstance == false)
            {
                isWarnedNullInstance = true;
                Debug.LogError(
                    ""
                    + $"{nameof(Game)} is not initialized, make sure you've created a "
                    + $"Resources/Game.prefab and added a script which inherits Game.cs"
                );

                UnityEditor.EditorApplication.isPaused = true;
            }

            return false;
#else
            return true;
#endif
        }
    }
}
