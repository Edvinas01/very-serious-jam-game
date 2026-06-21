using DoubleD.VerySeriousJamGame.Runtime.Pausing;
using InSun.GameCore;
using InSun.GameCore.Cursors;
using Unity.Cinemachine;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PlayerActor : MonoBehaviour
    {
        [SerializeField]
        private CinemachineCamera playerCamera;

        [SerializeField]
        private Transform forwardTransform;

        private CursorSystem cursorSystem;

        private bool isInteractionEnabled;
        private ICursor cursor;

        private void Awake()
        {
            cursorSystem = Game.GetObject<CursorSystem>();
        }

        private void OnEnable()
        {
            Game.AddListener<PauseStateChangedMessage>(OnPauseStateChanged);
        }

        private void OnDisable()
        {
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
