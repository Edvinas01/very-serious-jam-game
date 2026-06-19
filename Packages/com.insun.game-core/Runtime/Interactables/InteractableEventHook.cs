using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Interactables
{
    internal sealed class InteractableEventHook : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent<InteractableHoverEnteredArgs> onHoverEntered;

        [SerializeField]
        private UnityEvent<InteractableHoverExitedArgs> onHoverExited;

        [SerializeField]
        private UnityEvent<InteractableInteractionEnteredArgs> onSelectEntered;

        [SerializeField]
        private UnityEvent<InteractableInteractionExitedArgs> onSelectExited;

        private IInteractable interactable;

        private void Awake()
        {
            interactable = GetComponentInParent<IInteractable>();

            if (interactable == null)
            {
                Debug.LogError($"Must be placed on or under an {nameof(IInteractable)}", this);
                enabled = false;
            }
        }

        private void OnEnable()
        {
            interactable.OnHoverEntered += OnHoverEntered;
            interactable.OnHoverExited += OnHoverExited;
            interactable.OnInteractionEntered += OnInteractionEntered;
            interactable.OnInteractionExited += OnInteractionExited;
        }

        private void OnDisable()
        {
            interactable.OnHoverEntered -= OnHoverEntered;
            interactable.OnHoverExited -= OnHoverExited;
            interactable.OnInteractionEntered -= OnInteractionEntered;
            interactable.OnInteractionExited -= OnInteractionExited;
        }

        private void OnHoverEntered(InteractableHoverEnteredArgs args)
        {
            onHoverEntered.Invoke(args);
        }

        private void OnHoverExited(InteractableHoverExitedArgs args)
        {
            onHoverExited.Invoke(args);
        }

        private void OnInteractionEntered(InteractableInteractionEnteredArgs args)
        {
            onSelectEntered.Invoke(args);
        }

        private void OnInteractionExited(InteractableInteractionExitedArgs args)
        {
            onSelectExited.Invoke(args);
        }
    }
}
