using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Animations
{
    internal sealed class AnimationSequence : TweenAnimation
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Animation", Expanded = true)]
#else
        [Header("Animation")]
#endif
        [SerializeField]
        private List<TweenAnimation> sequence;

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

        public override bool IsPlaying
        {
            get
            {
                foreach (var entry in sequence)
                {
                    if (entry.IsPlaying)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

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
            PlayAsync(cancellationTokenSource.Token).Forget();
        }

        protected override async UniTask OnPlayAsync(CancellationToken cancellationToken = default)
        {
            OnPlayEntered?.Invoke();
            onPlayEntered.Invoke();

            foreach (var entry in sequence)
            {
                await entry.PlayAsync(cancellationToken);
            }

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
            foreach (var entry in sequence)
            {
                entry.Snap();
            }
        }
    }
}
