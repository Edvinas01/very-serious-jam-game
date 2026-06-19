using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Animations
{
    internal sealed class PauseAnimation : TweenAnimation
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Timings", Expanded = true)]
        [Sirenix.OdinInspector.MinValue(0f)]
#else
        [Header("Timings")]
        [Min(0f)]
#endif
        [SerializeField]
        private float duration = 1f;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events", Expanded = true)]
#else
        [Header("Events")]
#endif
        [SerializeField]
        private UnityEvent onPlayEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
#endif
        [SerializeField]
        private UnityEvent onPlayExited;

        private CancellationTokenSource cancellationTokenSource;
        private UniTask activeTask;

        public override bool IsPlaying => activeTask.Status == UniTaskStatus.Pending;

        public override Vector2 Duration
        {
            get => Vector2.zero;
            set { }
        }

        public override event Action OnPlayEntered;

        public override event Action OnPlayExited;

        protected override void OnPlay()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            activeTask = PlayAsync(cancellationTokenSource.Token);
        }

        protected override async UniTask OnPlayAsync(CancellationToken cancellationToken = default)
        {
            OnPlayEntered?.Invoke();
            onPlayEntered.Invoke();

            await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);

            OnPlayExited?.Invoke();
            onPlayExited.Invoke();
        }

        protected override void OnStop()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        protected override void OnSnap()
        {
        }
    }
}
