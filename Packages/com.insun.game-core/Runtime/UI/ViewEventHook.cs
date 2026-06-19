using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.UI
{
    internal sealed class ViewEventHook : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private View view;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onShowEntered;

        [SerializeField]
        private UnityEvent onShowExited;

        [SerializeField]
        private UnityEvent onHideEntered;

        [SerializeField]
        private UnityEvent onHideExited;

        private void Awake()
        {
            if (view == false)
            {
                view = GetComponentInParent<View>();
            }

            if (view == false)
            {
                Debug.LogWarning($"Must be placed under a {nameof(View)}", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            view.OnShowEntered += OnShowEntered;
            view.OnShowExited += OnShowExited;
            view.OnHideEntered += OnHideEntered;
            view.OnHideExited += OnHideExited;
        }

        private void OnDisable()
        {
            view.OnShowEntered -= OnShowEntered;
            view.OnShowExited -= OnShowExited;
            view.OnHideEntered -= OnHideEntered;
            view.OnHideExited -= OnHideExited;
        }

        private void OnShowEntered()
        {
            onShowEntered.Invoke();
        }

        private void OnShowExited()
        {
            onShowExited.Invoke();
        }

        private void OnHideEntered()
        {
            onHideEntered.Invoke();
        }

        private void OnHideExited()
        {
            onHideExited.Invoke();
        }
    }
}
