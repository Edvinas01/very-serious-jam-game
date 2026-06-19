using System.Collections.Generic;
using InSun.GameCore.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InSun.GameCore.Input
{
    public sealed class InputSystem : MonoBehaviour, ILifecycleListener
    {
        public enum ControlSchemeType
        {
            KeyboardMouse = 0,
            Gamepad = 1,
        }

        [Header("General")]
        [SerializeField]
        private PlayerInput playerInput;

        [Header("Control Schemes")]
        [SerializeField]
        private string keyboardMouseControlScheme = "KeyboardMouse";

        [SerializeField]
        private string gamepadControlScheme = "Gamepad";

        [Header("Action Maps")]
        [SerializeField]
        private string playerActionMapName = "Player";

        [SerializeField]
        private string uiActionMapName = "UI";

        public ControlSchemeType ControlScheme => GetControlScheme(playerInput);

        public bool IsPlayerInputEnabled { get; private set; }

        public bool IsUIInputEnabled { get; private set; }

        public void OnInitialized()
        {
            playerInput.onControlsChanged += OnControlsChanged;
            playerInput.enabled = true;
        }

        public void OnDisposed()
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }

        public void EnablePlayerInput()
        {
            foreach (var inputAction in GetActions(playerActionMapName))
            {
                inputAction.Enable();
            }

            IsPlayerInputEnabled = true;
        }

        public void DisablePlayerInput()
        {
            foreach (var inputAction in GetActions(playerActionMapName))
            {
                inputAction.Disable();
            }

            IsPlayerInputEnabled = false;
        }

        public void EnableUIInput()
        {
            foreach (var inputAction in GetActions(uiActionMapName))
            {
                inputAction.Enable();
            }

            IsUIInputEnabled = true;
        }

        public void DisableUIInput()
        {
            foreach (var inputAction in GetActions(uiActionMapName))
            {
                inputAction.Disable();
            }

            IsUIInputEnabled = false;
        }

        private void OnControlsChanged(PlayerInput input)
        {
            var controlScheme = GetControlScheme(input);
            var message = new ControlSchemeChangedMessage(controlScheme);

            Game.PublishMessage(message);
        }

        private IEnumerable<InputAction> GetActions(string actionMapName)
        {
            if (playerInput == false)
            {
                yield break;
            }

            var inputActions = playerInput.actions;
            if (inputActions == false)
            {
                yield break;
            }

            foreach (var inputActionMap in inputActions.actionMaps)
            {
                if (string.Equals(actionMapName, inputActionMap.name) == false)
                {
                    continue;
                }

                foreach (var inputAction in inputActionMap.actions)
                {
                    yield return inputAction;
                }
            }
        }

        private ControlSchemeType GetControlScheme(PlayerInput input)
        {
            if (input == false)
            {
                return ControlSchemeType.KeyboardMouse;
            }

            var schemeName = input.currentControlScheme;
            if (string.Equals(schemeName, keyboardMouseControlScheme))
            {
                return ControlSchemeType.KeyboardMouse;
            }

            if (string.Equals(schemeName, gamepadControlScheme))
            {
                return ControlSchemeType.Gamepad;
            }

            return ControlSchemeType.KeyboardMouse;
        }
    }
}
