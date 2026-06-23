using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace InSun.GameCore.Interactables
{
    public sealed class SimpleInteractor : MonoBehaviour, IInteractor
    {
        [SerializeField]
        private bool isSortHoveredByDistance = true;

        private readonly List<IInteractable> hoveredInteractables = new();
        private IInteractable closestHoveredInteractable;
        private IInteractable selectedInteractable;

        private void Update()
        {
            if (isSortHoveredByDistance && hoveredInteractables.Count > 0)
            {
                ReloadClosestHoveredInteractable();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null)
            {
                return;
            }

            if (hoveredInteractables.Contains(interactable))
            {
                return;
            }

            hoveredInteractables.Add(interactable);
            interactable.OnDestroyed += OnHoveredInteractableDestroyed;

            if (isSortHoveredByDistance)
            {
                ReloadClosestHoveredInteractable();
            }
            else
            {
                interactable.Hover(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null)
            {
                return;
            }

            if (hoveredInteractables.Remove(interactable) == false)
            {
                return;
            }

            interactable.OnDestroyed -= OnHoveredInteractableDestroyed;

            if (isSortHoveredByDistance)
            {
                if (interactable == closestHoveredInteractable)
                {
                    closestHoveredInteractable = null;
                    interactable.Unhover();
                }

                ReloadClosestHoveredInteractable();
            }
            else
            {
                interactable.Unhover();
            }
        }

        public bool IsInteractableHovered => hoveredInteractables.Count > 0;

        public bool IsInteracting => selectedInteractable != null;

        public event Action<IInteractable> OnInteractionEntered;
        public event Action<IInteractable> OnInteractionExited;

        public void StartInteraction()
        {
            var hoveredInteractable = isSortHoveredByDistance
                ? closestHoveredInteractable
                : hoveredInteractables.FirstOrDefault();

            if (hoveredInteractable == null)
            {
                return;
            }

            if (hoveredInteractable is SimpleInteractable simpleInteractable)
            {
                switch (simpleInteractable.InteractionType)
                {
                    case SimpleInteractable.SimpleInteractionType.None:
                    {
                        break;
                    }
                    case SimpleInteractable.SimpleInteractionType.Select:
                    {
                        if (selectedInteractable != null)
                        {
                            Debug.LogWarning("Interactable already selected");
                            return;
                        }

                        Select(hoveredInteractable);

                        break;
                    }
                    case SimpleInteractable.SimpleInteractionType.Activate:
                    {
                        Activate(hoveredInteractable);
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"{simpleInteractable.InteractionType} type not supported", this);
                        break;
                    }
                }
            }
            else
            {
                if (selectedInteractable != null)
                {
                    Debug.LogWarning("Interactable already selected");
                    return;
                }

                Select(hoveredInteractable);
            }
        }

        public void StopInteraction()
        {
            if (selectedInteractable == null)
            {
                return;
            }

            var interactablePrev = selectedInteractable;
            selectedInteractable = null;

            if (interactablePrev != null)
            {
                interactablePrev.StopInteraction();
                OnInteractionExited?.Invoke(interactablePrev);
            }
        }

        private void OnHoveredInteractableDestroyed(InteractableDestroyedArgs args)
        {
            args.Interactable.OnDestroyed -= OnHoveredInteractableDestroyed;
            if (!hoveredInteractables.Remove(args.Interactable))
            {
                return;
            }

            if (isSortHoveredByDistance)
            {
                if (args.Interactable == closestHoveredInteractable)
                {
                    closestHoveredInteractable = null;
                    args.Interactable.Unhover();
                }

                ReloadClosestHoveredInteractable();
            }
            else
            {
                args.Interactable.Unhover();
            }
        }

        private void Select(IInteractable interactable)
        {
            selectedInteractable = interactable;
            selectedInteractable?.StartInteraction(this);

            var isSelecting = IsInteracting;
            if (isSelecting)
            {
                OnInteractionEntered?.Invoke(selectedInteractable);
            }
        }

        private void Activate(IInteractable interactable)
        {
            interactable?.StartInteraction(this);

            if (interactable != null)
            {
                OnInteractionEntered?.Invoke(interactable);
                OnInteractionExited?.Invoke(interactable);
            }
        }

        private void ReloadClosestHoveredInteractable()
        {
            hoveredInteractables.Sort(CompareByDistanceToInteractor);

            var closest = hoveredInteractables.FirstOrDefault();

            if (closest == closestHoveredInteractable)
            {
                return;
            }

            closestHoveredInteractable?.Unhover();
            closestHoveredInteractable = closest;
            closestHoveredInteractable?.Hover(this);
        }

        private int CompareByDistanceToInteractor(IInteractable a, IInteractable b)
        {
            if (a is not Component aComp || b is not Component bComp)
            {
                return 0;
            }

            var posA = aComp.transform.position;
            var posB = bComp.transform.position;

            var dstA = (transform.position - posA).sqrMagnitude;
            var dstB = (transform.position - posB).sqrMagnitude;

            return dstA.CompareTo(dstB);
        }
    }
}
