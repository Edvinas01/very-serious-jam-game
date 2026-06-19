using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InSun.GameCore.UI
{
    public interface IView
    {
        public bool IsInteractable { get; set; }

        public bool IsCanvasEnabled { get; set; }

        public int SortingOrder { get; set; }

        public RectTransform CanvasRect { get; }

        public ViewState State { get; }

        public GameObject GameObjectToSelect { get; }

        public event Action OnShowEntered;

        public event Action OnShowExited;

        public event Action OnHideEntered;

        public event Action OnHideExited;

        public void Show();

        public void Hide();

        public void Show(bool isAnimate);

        public void Hide(bool isAnimate);

        public UniTask ShowAsync(CancellationToken cancellationToken = default);

        public UniTask HideAsync(CancellationToken cancellationToken = default);
    }
}
