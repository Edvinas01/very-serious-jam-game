using System;
using InSun.GameCore.SunnyInspector;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Interactables
{
    public sealed class SimpleInteractable : MonoBehaviour, IInteractable
    {
        public enum SimpleInteractionType
        {
            None = 0,
            Select = 1,
            Activate = 2,
        }

        [Header("Features")]
        [SerializeField]
        private bool isHoverEnabled = true;

        [SerializeField]
        private bool isSelectEnabled = true;

        [SerializeField]
        private SimpleInteractionType interactionType = SimpleInteractionType.Select;

        [Header("Events")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isHoverEnabled))]
#else
        [ShowIf(nameof(isHoverEnabled))]
#endif
        [SerializeField]
        private UnityEvent onHoverEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isHoverEnabled))]
#else
        [ShowIf(nameof(isHoverEnabled))]
#endif
        [SerializeField]
        private UnityEvent onHoverExited;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isSelectEnabled))]
#else
        [ShowIf(nameof(isSelectEnabled))]
#endif
        [SerializeField]
        private UnityEvent onSelectEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isSelectEnabled))]
#else
        [ShowIf(nameof(isSelectEnabled))]
#endif
        [SerializeField]
        private UnityEvent onSelectExited;

        private IInteractor hoveringInteractor;
        private IInteractor selectingInteractor;

        public bool IsHovered => hoveringInteractor != null;

        public bool IsSelected => selectingInteractor != null;

        public SimpleInteractionType InteractionType => interactionType;

        public event Action<InteractableHoverEnteredArgs> OnHoverEntered;

        public event Action<InteractableHoverExitedArgs> OnHoverExited;

        public event Action<InteractableInteractionEnteredArgs> OnInteractionEntered;

        public event Action<InteractableInteractionExitedArgs> OnInteractionExited;
        public event Action<InteractableDestroyedArgs> OnDestroyed;

        private void OnDisable()
        {
            if (Game.IsQuitting)
            {
                return;
            }

            Unhover();
            OnDestroyed?.Invoke(new InteractableDestroyedArgs(this, hoveringInteractor));
        }

        public bool Hover(IInteractor interactor)
        {
            if (isHoverEnabled == false)
            {
                return false;
            }

            hoveringInteractor = interactor;

            OnHoverEntered?.Invoke(new InteractableHoverEnteredArgs(this, hoveringInteractor));
            onHoverEntered.Invoke();

            return true;
        }

        public bool Unhover()
        {
            if (isHoverEnabled == false)
            {
                return false;
            }

            var interactorPrev = hoveringInteractor;
            if (interactorPrev == null)
            {
                return false;
            }

            hoveringInteractor = null;

            OnHoverExited?.Invoke(new InteractableHoverExitedArgs(this, interactorPrev));
            onHoverExited.Invoke();

            return true;
        }

        public bool StartInteraction(IInteractor interactor)
        {
            if (isSelectEnabled == false)
            {
                return false;
            }

            selectingInteractor = interactor;

            OnInteractionEntered?.Invoke(new InteractableInteractionEnteredArgs(this, interactor));
            onSelectEntered.Invoke();

            return true;
        }

        public bool StopInteraction()
        {
            var interactorPrev = selectingInteractor;
            if (interactorPrev == null)
            {
                return false;
            }

            selectingInteractor = null;

            interactorPrev.StopInteraction();

            OnInteractionExited?.Invoke(new InteractableInteractionExitedArgs(this, interactorPrev));
            onSelectExited.Invoke();

            return true;
        }
    }
}
