using System;
using System.Collections.Generic;

namespace InSun.GameCore.UI
{
    internal interface IViewStack
    {
        public event Action<IViewController> OnPushed;

        public event Action<IViewController> OnPopped;

        public bool IsClosedThisFrame { get; }

        public bool IsStackEmpty { get; }

        public IReadOnlyList<IViewController> Stack { get; }

        public void HideActive();

        public void NavigateTo(IViewController from, IViewController to);

        public void Push(IViewController controller);

        public void Pop(IViewController controller);
    }
}
