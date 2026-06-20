using System;
using DoubleD.VerySeriousJamGame.Runtime.Movement;
using DoubleD.VerySeriousJamGame.Runtime.Pausing;
using InSun.GameCore;
using InSun.GameCore.Cursors;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PlayerActor : MonoBehaviour
    {
        [Header("Scoring")]
        [Min(0)]
        [SerializeField]
        private int startingScore = 50;

        [Min(0)]
        [SerializeField]
        private int maxScore = 100;

        [SerializeField]
        private CinemachineCamera playerCamera;

        [SerializeField]
        private Transform forwardTransform;

        private GameplaySystem gameplaySystem;
        private CursorSystem cursorSystem;

        private bool isInteractionEnabled;
        private int currentScore;
        private ICursor cursor;

        public int MaxScore => maxScore;

        public int Score
        {
            get => currentScore;
            set
            {
                var valuePrev = currentScore;
                var valueNext = Mathf.Clamp(value, 0, maxScore);

                if (valuePrev == valueNext)
                {
                    return;
                }

                currentScore = valueNext;

                OnScoreChanged?.Invoke(valuePrev, valueNext);
            }
        }

        public event Action<int, int> OnScoreChanged;

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
            cursorSystem = Game.GetObject<CursorSystem>();

            currentScore = startingScore;
        }

        private void OnEnable()
        {
            Game.AddObject<int, PlayerActor>(GetInstanceID(), this);

            Game.AddListener<PauseStateChangedMessage>(OnPauseStateChanged);
        }

        private void OnDisable()
        {
            Game.RemoveObject<int, PlayerActor>(GetInstanceID());

            Game.RemoveListener<PauseStateChangedMessage>(OnPauseStateChanged);
        }

        private void Start()
        {
            cursor = cursorSystem.PushCursor((Sprite)null);
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void OnDestroy()
        {
            cursor?.Dispose();
            cursor = null;

            Cursor.lockState = CursorLockMode.None;
        }

        private void Update()
        {
            if (isInteractionEnabled == false)
            {
                return;
            }

            forwardTransform.rotation = playerCamera.transform.rotation;
        }

        public void EnableCamera()
        {
            playerCamera.enabled = true;
        }

        public void DisableCamera()
        {
            playerCamera.enabled = false;
        }

        public void EnableInteraction()
        {
            isInteractionEnabled = true;
        }

        public void DisableInteraction()
        {
            isInteractionEnabled = false;
        }

        private void OnPauseStateChanged(PauseStateChangedMessage message)
        {
            if (message.IsPausedNext)
            {
                cursor.Dispose();
                cursor = null;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                cursor = cursorSystem.PushCursor((Sprite)null);
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}
