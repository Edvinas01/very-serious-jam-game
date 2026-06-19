namespace InSun.GameCore.Interactables
{
    public readonly struct InteractableDestroyedArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractableDestroyedArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractableHoverEnteredArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractableHoverEnteredArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractableHoverExitedArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractableHoverExitedArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractableInteractionEnteredArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractableInteractionEnteredArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }

    public readonly struct InteractableInteractionExitedArgs
    {
        public IInteractable Interactable { get; }

        public IInteractor Interactor { get; }

        public InteractableInteractionExitedArgs(IInteractable interactable, IInteractor interactor)
        {
            Interactable = interactable;
            Interactor = interactor;
        }
    }
}
