using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InSun.GameCore.Input
{
    public abstract class InputActionListener<T> : InputActionListener where T : struct
    {
        [Header("General")]
        [SerializeField]
        private InputActionReference inputActionReference;

        [SerializeField]
        private bool isEnableAutomatically;

        [Header("Events")]
        [SerializeField]
        private UnityEvent<T> onPerformed;

        [SerializeField]
        private UnityEvent<T> onCanceled;

        public T Value => ReadValue();

        protected InputAction InputAction => inputActionReference.action;

        public event Action<T> OnPerformedValue;

        public event Action<T> OnCanceledValue;

        public override event Action OnPerformed;

        public override event Action OnCanceled;

        public override bool IsPressed => inputActionReference.action.IsPressed();

        public override bool IsPressedThisFrame => inputActionReference.action.WasPressedThisFrame();

        public override bool IsReleasedThisFrame => inputActionReference.action.WasReleasedThisFrame();

        protected override void Awake()
        {
            base.Awake();

            if (inputActionReference == false)
            {
                Debug.LogWarning("Input action is not set", this);
                enabled = false;
                return;
            }

            var action = inputActionReference.action;
            if (isEnableAutomatically)
            {
                action.Enable();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            var action = inputActionReference.action;
            action.performed += OnPerformedInternal;
            action.canceled += OnCancelledInternal;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var action = inputActionReference.action;
            action.performed -= OnPerformedInternal;
            action.canceled -= OnCancelledInternal;
        }

        protected virtual T ReadValue(InputAction.CallbackContext ctx)
        {
            return ctx.ReadValue<T>();
        }

        protected virtual T ReadValue()
        {
            return InputAction.ReadValue<T>();
        }

        private void OnPerformedInternal(InputAction.CallbackContext ctx)
        {
            var value = ReadValue(ctx);

            OnPerformedValue?.Invoke(value);
            OnPerformed?.Invoke();
            onPerformed.Invoke(value);
        }

        private void OnCancelledInternal(InputAction.CallbackContext ctx)
        {
            var value = ReadValue(ctx);

            OnCanceledValue?.Invoke(value);
            OnCanceled?.Invoke();
            onCanceled.Invoke(value);
        }
    }

    public abstract class InputActionListener : MonoBehaviour
    {
        public abstract event Action OnPerformed;

        public abstract event Action OnCanceled;

        public abstract bool IsPressed { get; }

        public abstract bool IsReleasedThisFrame { get; }

        public abstract bool IsPressedThisFrame { get; }

        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }
    }
}
