using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InSun.GameCore.UI
{
    public interface IViewController
    {
        public bool IsClosable { get; }

        public bool IsBlockingInput { get; }

        public bool IsInteractable { get; set; }

        public int SortingOrder { get; }

        public GameObject GameObjectToSelect { get; }

        public event Action OnShowEntered;

        public event Action OnShowExited;

        public event Action OnHideEntered;

        public event Action OnHideExited;

        public void ShowView();

        public void HideView();

        public UniTask ShowViewAsync(CancellationToken cancellationToken = default);

        public UniTask HideViewAsync(CancellationToken cancellationToken = default);

        public void ToggleVisibility();

        public UniTask ToggleVisibilityAsync(CancellationToken cancellationToken = default);

        public void NavigateTo(IViewController target);

        public UniTask NavigateToAsync(IViewController target, CancellationToken cancellationToken = default);
    }
}
