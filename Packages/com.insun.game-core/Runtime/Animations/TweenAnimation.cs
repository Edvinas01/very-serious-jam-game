using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace InSun.GameCore.Animations
{
    public abstract class TweenAnimation : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug", Expanded = true)]
        [Sirenix.OdinInspector.ReadOnly]
        [Sirenix.OdinInspector.ShowInInspector]
#endif
        public abstract bool IsPlaying { get; }

        public abstract Vector2 Duration { get; set; }

        public abstract event Action OnPlayEntered;

        public abstract event Action OnPlayExited;

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug", Expanded = true)]
        [Sirenix.OdinInspector.HorizontalGroup("Debug/Actions")]
        [Sirenix.OdinInspector.DisableInEditorMode]
        [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Medium)]
#endif
        [ContextMenu("Play")]
        public void Play()
        {
            OnPlay();
        }

        public UniTask PlayAsync(CancellationToken cancellationToken = default)
        {
            return OnPlayAsync(cancellationToken);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Debug/Actions")]
        [Sirenix.OdinInspector.DisableInEditorMode]
        [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Medium)]
#endif
        [ContextMenu("Stop")]
        public void Stop()
        {
            OnStop();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Debug/Actions")]
        [Sirenix.OdinInspector.DisableInEditorMode]
        [Sirenix.OdinInspector.Button(Sirenix.OdinInspector.ButtonSizes.Medium)]
#endif
        [ContextMenu("Snap")]
        public void Snap()
        {
            OnStop();
            OnSnap();
        }

        protected abstract void OnPlay();

        protected abstract UniTask OnPlayAsync(CancellationToken cancellationToken = default);

        protected abstract void OnStop();

        protected abstract void OnSnap();
    }
}
