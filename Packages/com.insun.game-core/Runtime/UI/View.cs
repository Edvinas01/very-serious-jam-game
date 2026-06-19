using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using InSun.GameCore.Animations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace InSun.GameCore.UI
{
    public abstract class View : MonoBehaviour, IView
    {
        [Serializable]
        private sealed class ViewData
        {
            [Header("UI")]
            [SerializeField]
            private Canvas canvas;

            [SerializeField]
            private CanvasGroup canvasGroup;

            [Header("Selection")]
            [SerializeField]
            private GameObject gameObjectToSelect;

            [FormerlySerializedAs("showAnimator")]
            [Header("Animation")]
            [SerializeField]
            private TweenAnimation showAnimation;

            [FormerlySerializedAs("hideAnimator")]
            [SerializeField]
            private TweenAnimation hideAnimation;

            [Header("Events")]
            [SerializeField]
            private UnityEvent onShowEntered;

            [SerializeField]
            private UnityEvent onShowExited;

            [SerializeField]
            private UnityEvent onHideEntered;

            [SerializeField]
            private UnityEvent onHideExited;

            public Canvas Canvas
            {
                get => canvas;
                set => canvas = value;
            }

            public CanvasGroup CanvasGroup
            {
                get => canvasGroup;
                set => canvasGroup = value;
            }

            public int SortingOrder
            {
                get => canvas.sortingOrder;
                set => canvas.sortingOrder = value;
            }

            public GameObject GameObjectToSelect
            {
                get => gameObjectToSelect;
                set => gameObjectToSelect = value;
            }

            public TweenAnimation ShowAnimation => showAnimation;

            public TweenAnimation HideAnimation => hideAnimation;

            public UnityEvent OnShowEntered => onShowEntered;

            public UnityEvent OnShowExited => onShowExited;

            public UnityEvent OnHideEntered => onHideEntered;

            public UnityEvent OnHideExited => onHideExited;
        }

        [SerializeField]
        private ViewData viewData = new();

        private bool isBlocksRaycasts;
        private bool isShownOnce;

        public virtual GameObject GameObjectToSelect => viewData.GameObjectToSelect;

        protected TweenAnimation ShowAnimation => viewData.ShowAnimation;

        protected TweenAnimation HideAnimation => viewData.HideAnimation;

        public bool IsInteractable
        {
            get => viewData.CanvasGroup.interactable;
            set
            {
                var group = viewData.CanvasGroup;
                group.interactable = value;

                if (isBlocksRaycasts)
                {
                    group.blocksRaycasts = value;
                }
            }
        }

        public bool IsCanvasEnabled
        {
            get => viewData.Canvas.enabled;
            set => viewData.Canvas.enabled = value;
        }

        public int SortingOrder
        {
            get => viewData.SortingOrder;
            set => viewData.SortingOrder = value;
        }

        public RectTransform CanvasRect => (RectTransform)viewData.Canvas.transform;

        public ViewState State
        {
            get
            {
                if (viewData.ShowAnimation && viewData.ShowAnimation.IsPlaying)
                {
                    return ViewState.Showing;
                }

                if (viewData.HideAnimation && viewData.HideAnimation.IsPlaying)
                {
                    return ViewState.Hiding;
                }

                if (viewData.Canvas.enabled)
                {
                    return ViewState.Shown;
                }

                return ViewState.Hidden;
            }
        }

        public event Action OnShowEntered;

        public event Action OnShowExited;

        public event Action OnHideEntered;

        public event Action OnHideExited;

#if UNITY_EDITOR
        protected void Reset()
        {
            viewData.Canvas = GetComponent<Canvas>();
            if (viewData.Canvas == false)
            {
                viewData.Canvas = gameObject.AddComponent<Canvas>();
            }

            viewData.CanvasGroup = GetComponent<CanvasGroup>();
            if (viewData.CanvasGroup == false)
            {
                viewData.CanvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
#endif

        protected virtual void Awake()
        {
            if (viewData.Canvas == false)
            {
                viewData.Canvas = GetComponent<Canvas>();
            }

            if (viewData.Canvas == false)
            {
                Debug.LogError($"{nameof(viewData.Canvas)} must be set", this);
                enabled = false;
                return;
            }

            if (viewData.CanvasGroup == false)
            {
                viewData.CanvasGroup = GetComponent<CanvasGroup>();
            }

            if (viewData.CanvasGroup == false)
            {
                Debug.LogError($"{nameof(viewData.CanvasGroup)} must be set", this);
                enabled = false;
                return;
            }

            isBlocksRaycasts = viewData.CanvasGroup.blocksRaycasts;
        }

        protected virtual void OnEnable()
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.OnPlayEntered += OnViewShowEntered;
                viewData.ShowAnimation.OnPlayExited += OnViewShowExited;
            }

            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.OnPlayEntered += OnViewHideEntered;
                viewData.HideAnimation.OnPlayExited += OnViewHideExited;
            }
        }

        protected virtual void OnDisable()
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.OnPlayEntered -= OnViewShowEntered;
                viewData.ShowAnimation.OnPlayExited -= OnViewShowExited;
            }

            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.OnPlayEntered -= OnViewHideEntered;
                viewData.HideAnimation.OnPlayExited -= OnViewHideExited;
            }
        }

        protected virtual void Start()
        {
        }

        public virtual void Show()
        {
            Show(isAnimate: true);
        }

        public virtual void Hide()
        {
            Hide(isAnimate: true);
        }

        public virtual void Show(bool isAnimate)
        {
            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.Stop();
            }

            if (isAnimate && viewData.ShowAnimation)
            {
                if (isShownOnce == false && viewData.HideAnimation)
                {
                    viewData.HideAnimation.Snap();
                }

                viewData.ShowAnimation.Play();
            }
            else
            {
                OnViewShowEntered();
                OnViewShowExited();
            }

            isShownOnce = true;
        }

        public virtual void Hide(bool isAnimate)
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.Stop();
            }

            if (isAnimate && viewData.HideAnimation)
            {
                viewData.HideAnimation.Play();
            }
            else
            {
                OnViewHideEntered();
                OnViewHideExited();
            }
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if (viewData.HideAnimation)
            {
                viewData.HideAnimation.Stop();
            }

            if (viewData.ShowAnimation)
            {
                if (isShownOnce == false && viewData.HideAnimation)
                {
                    viewData.HideAnimation.Snap();
                }

                await viewData.ShowAnimation.PlayAsync(cancellationToken);
            }
            else
            {
                OnViewShowEntered();
                OnViewShowExited();
            }

            isShownOnce = true;
        }

        public virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            if (viewData.ShowAnimation)
            {
                viewData.ShowAnimation.Stop();
            }

            if (viewData.HideAnimation)
            {
                await viewData.HideAnimation.PlayAsync(cancellationToken);
            }
            else
            {
                OnViewHideEntered();
                OnViewHideExited();
            }
        }

        protected virtual void OnViewShowEntered()
        {
            IsCanvasEnabled = true;
            IsInteractable = true;

            OnShowEntered?.Invoke();
            viewData.OnShowEntered.Invoke();
        }

        protected virtual void OnViewShowExited()
        {
            OnShowExited?.Invoke();
            viewData.OnShowExited.Invoke();
        }

        protected virtual void OnViewHideEntered()
        {
            IsInteractable = false;

            OnHideEntered?.Invoke();
            viewData.OnHideEntered.Invoke();
        }

        protected virtual void OnViewHideExited()
        {
            IsCanvasEnabled = false;

            OnHideExited?.Invoke();
            viewData.OnHideExited.Invoke();
        }
    }
}
