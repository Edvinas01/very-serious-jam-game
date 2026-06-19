using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InSun.GameCore.SunnyInspector;
using UnityEngine;
using static InSun.GameCore.UI.ViewState;

namespace InSun.GameCore.UI
{
    public abstract class ViewController<TView> : ViewController where TView : View
    {
        [Serializable]
        private sealed class ControllerData
        {
            [Header("View")]
            [SerializeField]
            private TView view;

            [SerializeField]
            private Transform viewParent;

            [SerializeField]
            private StartMode startMode = StartMode.Show;

            [Header("Stack")]
            [SerializeField]
            private bool isTrackedByStack;

            [ShowIf(nameof(isTrackedByStack))]
            [SerializeField]
            private bool isClosable = true;

            [ShowIf(nameof(isTrackedByStack))]
            [SerializeField]
            private bool isBlockingInput;

            [Header("Order")]
            [SerializeField]
            private bool isOverrideSortingOrder;

            [ShowIf(nameof(isOverrideSortingOrder))]
            [SerializeField]
            private ViewOrder viewOrder;

            [ShowIf(nameof(IsShowSortingOrderEditor))]
            [Min(0)]
            [SerializeField]
            private int sortingOrder;

            public bool IsTrackedByStack => isTrackedByStack;

            public bool IsClosable => isClosable;

            public bool IsBlockingInput => isBlockingInput;

            public TView View
            {
                get => view;
                set => view = value;
            }

            public Transform ViewParent
            {
                get => viewParent;
                set => viewParent = value;
            }

            public StartMode StartMode => startMode;

            public int SortingOrder
            {
                get
                {
                    if (isOverrideSortingOrder)
                    {
                        if (viewOrder)
                        {
                            return viewOrder.SortingOrder;
                        }

                        return sortingOrder;
                    }

                    return View.SortingOrder;
                }
            }

            private bool IsShowSortingOrderEditor()
            {
                return isOverrideSortingOrder && viewOrder == false;
            }
        }

        private enum StartMode
        {
            None = 0,
            Show = 1,
            Hide = 2,
        }

        [SerializeField]
        private ControllerData controllerData = new();

        private bool isViewVisibilityModified;
        private TView currentView;

        public override bool IsClosable => controllerData.IsClosable;

        public override bool IsBlockingInput => controllerData.IsBlockingInput;

        public override int SortingOrder => View.SortingOrder;

        public override bool IsInteractable
        {
            get => View.IsInteractable;
            set => View.IsInteractable = value;
        }

        public TView View
        {
            protected get
            {
                if (currentView == false)
                {
                    currentView = CreateView();
                }

                return currentView;
            }
            set => controllerData.View = value;
        }

        public ViewState ViewState => View.State;

#if UNITY_EDITOR
        protected void Reset()
        {
            controllerData.ViewParent = transform;
        }
#endif

        protected virtual void Awake()
        {
            var view = controllerData.View;
            if (view == false)
            {
                Debug.LogError($"{nameof(controllerData.View)} must be set to a prefab or an instance", this);
                enabled = false;
                return;
            }

            if (currentView)
            {
                return;
            }

            currentView = view.gameObject.scene.IsValid() ? view : CreateView();

            currentView.OnShowEntered += OnViewShowEntered;
            currentView.OnShowExited += OnViewShowExited;
            currentView.OnHideEntered += OnViewHideEntered;
            currentView.OnHideExited += OnViewHideExited;
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void Start()
        {
            View.SortingOrder = controllerData.SortingOrder;

            if (isViewVisibilityModified)
            {
                return;
            }

            switch (controllerData.StartMode)
            {
                case StartMode.Show:
                {
                    OnViewShowEntered();
                    View.IsCanvasEnabled = true;
                    View.IsInteractable = true;
                    OnViewShowExited();

                    break;
                }
                case StartMode.Hide:
                {
                    OnViewHideEntered();
                    View.IsCanvasEnabled = false;
                    View.IsInteractable = false;
                    OnViewHideExited();

                    break;
                }
                case StartMode.None:
                default:
                {
                    break;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            currentView.OnShowEntered -= OnViewShowEntered;
            currentView.OnShowExited -= OnViewShowExited;
            currentView.OnHideEntered -= OnViewHideEntered;
            currentView.OnHideExited -= OnViewHideExited;

            ViewStack.Instance?.Pop(this);

            currentView = null;
        }

        protected virtual void Update()
        {
        }

        internal override void InitializeView()
        {
            if (currentView)
            {
                return;
            }

#if UNITY_EDITOR
            var undoIndex = UnityEditor.Undo.GetCurrentGroup();
            UnityEditor.Undo.SetCurrentGroupName(nameof(InitializeView));
            UnityEditor.Undo.RecordObject(this, nameof(InitializeView));
#endif
            currentView = CreateView();
            OnViewInitialized();

#if UNITY_EDITOR
            UnityEditor.Undo.CollapseUndoOperations(undoIndex);
#endif
        }

        public override void ShowView()
        {
            isViewVisibilityModified = true;
            View.Show();
        }

        public override void HideView()
        {
            isViewVisibilityModified = true;
            View.Hide();
        }

        public override UniTask ShowViewAsync(CancellationToken cancellationToken = default)
        {
            isViewVisibilityModified = true;
            return View.ShowAsync(cancellationToken);
        }

        public override UniTask HideViewAsync(CancellationToken cancellationToken = default)
        {
            isViewVisibilityModified = true;
            return View.HideAsync(cancellationToken);
        }

        public override void ToggleVisibility()
        {
            isViewVisibilityModified = true;

            if (View.State is Showing or Shown)
            {
                View.Hide();
            }
            else
            {
                View.Show();
            }
        }

        public override UniTask ToggleVisibilityAsync(CancellationToken cancellationToken = default)
        {
            isViewVisibilityModified = true;

            if (View.State is Showing or Shown)
            {
                return View.HideAsync(cancellationToken);
            }

            return View.ShowAsync(cancellationToken);
        }

        public override GameObject GameObjectToSelect => View.GameObjectToSelect;

        protected override void OnViewShowEntered()
        {
            if (controllerData.IsTrackedByStack)
            {
                ViewStack.Instance?.Push(this);
            }

            base.OnViewShowEntered();
        }

        protected override void OnViewHideEntered()
        {
            base.OnViewHideEntered();

            if (controllerData.IsTrackedByStack)
            {
                ViewStack.Instance?.Pop(this);
            }
        }

        protected virtual void OnViewInitialized()
        {
        }

        private TView CreateView()
        {
            var parent = controllerData.ViewParent ? controllerData.ViewParent : transform;
            var view = controllerData.View;

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                var newView = (TView)UnityEditor.PrefabUtility.InstantiatePrefab(view, parent);
                UnityEditor.Undo.RegisterCreatedObjectUndo(
                    newView.gameObject,
                    $"Create view {newView.name}"
                );

                newView.name = view.name;
                return newView;
            }
#endif

            var instance = Instantiate(view, parent);
            instance.name = view.name;

            return instance;
        }
    }

    public abstract class ViewController : MonoBehaviour, IViewController
    {
        public abstract bool IsClosable { get; }

        public abstract bool IsBlockingInput { get; }

        public abstract bool IsInteractable { get; set; }

        public abstract int SortingOrder { get; }

        public abstract GameObject GameObjectToSelect { get; }

        public event Action OnShowEntered;

        public event Action OnShowExited;

        public event Action OnHideEntered;

        public event Action OnHideExited;

        public abstract void ShowView();

        public abstract void HideView();

        public abstract UniTask ShowViewAsync(CancellationToken cancellationToken = default);

        public abstract UniTask HideViewAsync(CancellationToken cancellationToken = default);

        public abstract void ToggleVisibility();

        public abstract UniTask ToggleVisibilityAsync(CancellationToken cancellationToken = default);

        internal abstract void InitializeView();

        public void NavigateTo(IViewController target)
        {
            ViewStack.Instance?.NavigateTo(this, target);
        }

        public UniTask NavigateToAsync(IViewController target, CancellationToken cancellationToken = default)
        {
            return ViewStack.Instance?.NavigateToAsync(this, target, cancellationToken) ?? UniTask.CompletedTask;
        }

        protected virtual void OnViewShowEntered()
        {
            OnShowEntered?.Invoke();
        }

        protected virtual void OnViewShowExited()
        {
            OnShowExited?.Invoke();
        }

        protected virtual void OnViewHideEntered()
        {
            OnHideEntered?.Invoke();
        }

        protected virtual void OnViewHideExited()
        {
            OnHideExited?.Invoke();
        }
    }
}
