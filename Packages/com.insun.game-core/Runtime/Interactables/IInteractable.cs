using System;

namespace InSun.GameCore.Interactables
{
    public interface IInteractable
    {
        public bool IsHovered { get; }

        public bool IsSelected { get; }

        public event Action<InteractableHoverEnteredArgs> OnHoverEntered;

        public event Action<InteractableHoverExitedArgs> OnHoverExited;

        public event Action<InteractableInteractionEnteredArgs> OnInteractionEntered;

        public event Action<InteractableInteractionExitedArgs> OnInteractionExited;

        public event Action<InteractableDestroyedArgs> OnDestroyed;

        public bool Hover(IInteractor interactor);

        public bool Unhover();

        public bool StartInteraction(IInteractor interactor);

        public bool StopInteraction();
    }
}
