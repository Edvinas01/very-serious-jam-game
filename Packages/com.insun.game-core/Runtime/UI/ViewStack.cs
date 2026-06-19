using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InSun.GameCore.UI
{
    public sealed class ViewStack : MonoBehaviour, IViewStack
    {
        [SerializeField]
        private bool isDontDestroyOnLoad = true;

        private readonly List<IViewController> stack = new();

        private readonly Dictionary<IViewController, IViewController> navigationTargets = new();
        private readonly Dictionary<IViewController, GameObject> savedSelections = new();

        public static ViewStack Instance { get; private set; }

        public GameObject PendingSelection { get; set; }

        public bool IsClosedThisFrame { get; private set; }

        public bool IsStackEmpty => stack.Count == 0;

        public IReadOnlyList<IViewController> Stack => stack;

        public event Action<IViewController> OnPushed;

        public event Action<IViewController> OnPopped;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (isDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void LateUpdate()
        {
            IsClosedThisFrame = false;

            if (PendingSelection)
            {
                var eventSystem = EventSystem.current;
                if (eventSystem.currentSelectedGameObject == PendingSelection)
                {
                    // Clear selection if we queue up the same obj, otherwise the obj wont
                    // receive a selection event
                    eventSystem.SetSelectedGameObject(null);
                }

                eventSystem.SetSelectedGameObject(PendingSelection);
                PendingSelection = null;
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void HideActive()
        {
            if (stack.Count <= 0)
            {
                return;
            }

            IViewController topController = null;
            for (var index = 0; index < stack.Count; index++)
            {
                var controller = stack[index];
                if (controller.IsClosable == false)
                {
                    continue;
                }

                if (topController == null || controller.SortingOrder >= topController.SortingOrder)
                {
                    topController = controller;
                }
            }

            if (topController == null)
            {
                return;
            }

            IsClosedThisFrame = true;

            if (navigationTargets.Remove(topController, out var returnTarget))
            {
                returnTarget.ShowView();
            }

            topController.HideView();
        }

        public void NavigateTo(IViewController from, IViewController to)
        {
            navigationTargets[to] = from;

            from.HideView();
            to.ShowView();
        }


        public UniTask NavigateToAsync(IViewController from, IViewController to, CancellationToken cancellationToken)
        {
            navigationTargets[to] = from;

            return UniTask.WhenAll(
                from.HideViewAsync(cancellationToken),
                to.ShowViewAsync(cancellationToken)
            );
        }

        public void Push(IViewController controller)
        {
            if (TryGetTopController(out var currentTop))
            {
                savedSelections[currentTop] = EventSystem.current.currentSelectedGameObject;
            }

            stack.Remove(controller);
            stack.Add(controller);

            OnPushed?.Invoke(controller);

            UpdateInteractability();
            UpdateFocus();
        }

        public void Pop(IViewController controller)
        {
            navigationTargets.Remove(controller);
            savedSelections.Remove(controller);

            if (stack.Remove(controller))
            {
                OnPopped?.Invoke(controller);

                UpdateInteractability();
                UpdateFocus();
            }
        }

        private void UpdateInteractability()
        {
            // Find the highest sort order blocking controller
            var blockingOrder = int.MinValue;
            for (var index = 0; index < stack.Count; index++)
            {
                var controller = stack[index];
                if (controller.IsBlockingInput && controller.SortingOrder > blockingOrder)
                {
                    blockingOrder = controller.SortingOrder;
                }
            }

            // Block anything visually below it
            for (var index = 0; index < stack.Count; index++)
            {
                var controller = stack[index];
                controller.IsInteractable = controller.SortingOrder >= blockingOrder;
            }
        }

        private void UpdateFocus()
        {
            if (stack.Count <= 0)
            {
                PendingSelection = null;
                EventSystem.current?.SetSelectedGameObject(null);
                return;
            }

            if (TryGetTopController(out var topController) == false)
            {
                return;
            }

            if (savedSelections.TryGetValue(topController, out var savedSelection) && savedSelection)
            {
                PendingSelection = savedSelection;
            }
            else
            {
                PendingSelection = topController.GameObjectToSelect;
            }
        }

        private bool TryGetTopController(out IViewController result)
        {
            IViewController topController = null;
            for (var index = 0; index < stack.Count; index++)
            {
                var controller = stack[index];
                if (topController == null || controller.SortingOrder >= topController.SortingOrder)
                {
                    topController = controller;
                }
            }

            if (topController == null)
            {
                result = null;
                return false;
            }

            result = topController;
            return true;
        }
    }
}
