using System;

namespace InSun.GameCore.Interactables
{
    public interface IInteractor
    {
        public bool IsInteractableHovered { get; }

        public bool IsInteracting { get; }

        public event Action<IInteractable> OnInteractionEntered;

        public event Action<IInteractable> OnInteractionExited;

        public void StartInteraction();

        public void StopInteraction();
    }
}
