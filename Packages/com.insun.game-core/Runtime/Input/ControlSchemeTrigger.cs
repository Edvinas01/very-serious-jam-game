using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Input
{
    public sealed class ControlSchemeTrigger : MonoBehaviour
    {
        [Header("Game Objects")]
        [SerializeField]
        private List<GameObject> keyboardMouseObjects;

        [SerializeField]
        private List<GameObject> gamepadObjects;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onKeyboardMouse;

        [SerializeField]
        private UnityEvent onGamepad;

        private InputSystem inputSystem;

        private void Awake()
        {
            inputSystem = Game.GetObject<InputSystem>();
        }

        private void OnEnable()
        {
            SetControlScheme(inputSystem.ControlScheme);
            Game.AddListener<ControlSchemeChangedMessage>(OnControlSchemeChanged);
        }

        private void OnDisable()
        {
            Game.RemoveListener<ControlSchemeChangedMessage>(OnControlSchemeChanged);
        }

        private void OnControlSchemeChanged(ControlSchemeChangedMessage message)
        {
            SetControlScheme(message.ControlScheme);
        }

        private void SetControlScheme(InputSystem.ControlSchemeType controlScheme)
        {
            switch (controlScheme)
            {
                case InputSystem.ControlSchemeType.KeyboardMouse:
                {
                    foreach (var obj in keyboardMouseObjects)
                    {
                        obj.SetActive(true);
                    }

                    foreach (var obj in gamepadObjects)
                    {
                        obj.SetActive(false);
                    }

                    onKeyboardMouse.Invoke();
                    break;
                }
                case InputSystem.ControlSchemeType.Gamepad:
                {
                    foreach (var obj in keyboardMouseObjects)
                    {
                        obj.SetActive(false);
                    }

                    foreach (var obj in gamepadObjects)
                    {
                        obj.SetActive(true);
                    }

                    onGamepad.Invoke();
                    break;
                }
                default:
                {
                    Debug.LogWarning($"Unsupported control scheme: {controlScheme}", this);
                    break;
                }
            }
        }
    }
}
