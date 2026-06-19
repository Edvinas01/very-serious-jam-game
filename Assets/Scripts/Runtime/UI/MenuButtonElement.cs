using System;
using InSun.GameCore.SunnyInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DoubleD.VerySeriousJamGame.Runtime.UI
{
    internal sealed class MenuButtonElement : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Header("Visuals")]
        [SerializeField]
        private CanvasGroup canvasGroup;

        [ShowIf(nameof(canvasGroup))]
        [Range(0f, 1f)]
        [SerializeField]
        private float disabledAlpha = 0.5f;

        [SerializeField]
        private Image selectionHighlight;

        [SerializeField]
        private TMP_Text contentText;

        [Header("Events: Clicks")]
        [SerializeField]
        private UnityEvent onClicked;

        [Header("Events: Hover")]
        [SerializeField]
        private UnityEvent onHoverEntered;

        [SerializeField]
        private UnityEvent onHoverExited;

        [Header("Events: Selection")]
        [SerializeField]
        private UnityEvent onSelectEntered;

        [SerializeField]
        private UnityEvent onSelectExited;

        public string Text
        {
            set => contentText.text = value;
        }

        public event Action OnClicked;
        public event Action OnHoverEntered;
        public event Action OnHoverExited;
        public event Action OnSelectEntered;
        public event Action OnSelectExited;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsInteractable() == false || IsActive() == false)
            {
                return;
            }

            OnClicked?.Invoke();
            onClicked.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (IsInteractable() == false || IsActive() == false)
            {
                return;
            }

            OnClicked?.Invoke();
            onClicked.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable() == false || IsActive() == false)
            {
                return;
            }

            if (selectionHighlight)
            {
                selectionHighlight.enabled = true;
            }

            base.OnPointerEnter(eventData);

            OnHoverEntered?.Invoke();
            onHoverEntered.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            if (selectionHighlight)
            {
                selectionHighlight.enabled = false;
            }

            OnHoverExited?.Invoke();
            onHoverExited.Invoke();
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (IsInteractable() == false || IsActive() == false)
            {
                return;
            }

            base.OnSelect(eventData);

            if (selectionHighlight)
            {
                selectionHighlight.enabled = true;
            }

            OnSelectEntered?.Invoke();
            onSelectEntered.Invoke();
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            if (selectionHighlight)
            {
                selectionHighlight.enabled = false;
            }

            OnSelectExited?.Invoke();
            onSelectExited.Invoke();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            canvasGroup.alpha = state == SelectionState.Disabled ? disabledAlpha : 1f;
        }
    }
}
