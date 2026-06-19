using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

#if PRIMETWEEN_INSTALLED
using PrimeTween;

#else
using DG.Tweening;
#endif

namespace InSun.GameCore.Animations
{
    internal sealed class CanvasAlphaAnimation : TweenAnimation
    {
        private enum AlphaMode
        {
            [Tooltip("Fade by using a fixed alpha to fade from/to")]
            FixedAlpha = 0,

            [Tooltip("Fade by multiplying the initial alpha by given value")]
            AlphaMultiplier = 1,

            [Tooltip("Fade from/to current canvas alpha")]
            CurrentAlpha = 2,

            [Tooltip("Fade from/to initial canvas alpha")]
            InitialAlpha = 3,
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private CanvasGroup target;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Alpha", Expanded = true)]
#else
        [Header("Alpha")]
#endif
        [SerializeField]
        private AlphaMode startAlphaMode = AlphaMode.FixedAlpha;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Alpha")]
        [Sirenix.OdinInspector.PropertyRange(0f, 1f)]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowStartAlphaEditor))]
#else
        [Range(0f, 1f)]
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(IsShowStartAlphaEditor))]
#endif
        [SerializeField]
        private float startAlpha;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Alpha")]
        [Sirenix.OdinInspector.PropertySpace]
#else
        [Space]
#endif
        [SerializeField]
        private AlphaMode endAlphaMode = AlphaMode.FixedAlpha;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Alpha")]
        [Sirenix.OdinInspector.PropertyRange(0f, 1f)]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowEndAlphaEditor))]
#else
        [Range(0f, 1f)]
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(IsShowEndAlphaEditor))]
#endif
        [SerializeField]
        private float endAlpha = 1f;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Tween", Expanded = true)]
#else
        [Header("Tween")]
#endif
        [SerializeField]
        private Ease ease = Ease.InOutCubic;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Tween")]
        [Sirenix.OdinInspector.MinValue(0f)]
#else
        [Min(0f)]
#endif
        [SerializeField]
        private Vector2 duration = new(0.5f, 0.5f);

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features", Expanded = true)]
#else
        [Header("Features")]
#endif
        [SerializeField]
        private bool isPlayOnStart;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features")]
#endif
        [SerializeField]
        private bool isPlayOnce;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features")]
#endif
        [SerializeField]
        private bool isDestroyOnCompleted;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features")]
#endif
        [Tooltip("Tween ignoring " + nameof(Time.timeScale) + "?")]
        [SerializeField]
        private bool isIndependentUpdate;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events", Expanded = true)]
#else
        [Header("Events")]
#endif
        [FormerlySerializedAs("onStarted")]
        [SerializeField]
        private UnityEvent onPlayEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
#endif
        [FormerlySerializedAs("onCompleted")]
        [SerializeField]
        private UnityEvent onPlayExited;

#if PRIMETWEEN_INSTALLED
        private Tween activeTween;
#else
        private Tween activeTween;
#endif

        private bool isTriggeredOnce;
        private float initialAlpha;

#if PRIMETWEEN_INSTALLED == false
        private string animationId;
#endif

        public override bool IsPlaying
        {
            get
            {
#if PRIMETWEEN_INSTALLED
                return activeTween.isAlive;
#else
                if (activeTween == null)
                {
                    return false;
                }

                if (activeTween.IsActive() == false)
                {
                    return false;
                }

                return activeTween.IsPlaying();
#endif
            }
        }

        public override Vector2 Duration
        {
            get => duration;
            set => duration = value;
        }

        public override event Action OnPlayEntered;

        public override event Action OnPlayExited;

#if UNITY_EDITOR
        private void Reset()
        {
            target = GetComponent<CanvasGroup>();
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            if (target == false)
            {
                target = GetComponent<CanvasGroup>();
            }

#if PRIMETWEEN_INSTALLED == false
            animationId = Guid.NewGuid().ToString();
#endif
        }

        protected override void Start()
        {
            base.Start();

            initialAlpha = target.alpha;

            if (isPlayOnStart)
            {
                Play();
            }
        }

        protected override void OnPlay()
        {
            if (isPlayOnce && isTriggeredOnce)
            {
                return;
            }

            isTriggeredOnce = true;

#if PRIMETWEEN_INSTALLED
            activeTween.Stop();
            activeTween = PlayTween();
#else
            DOTween.Kill(animationId);
            activeTween = PlayTween();
#endif
        }

        protected override async UniTask OnPlayAsync(CancellationToken cancellationToken = default)
        {
            if (isPlayOnce && isTriggeredOnce)
            {
                return;
            }

            isTriggeredOnce = true;

#if PRIMETWEEN_INSTALLED
            activeTween.Stop();
            activeTween = PlayTween();
            await activeTween.ToYieldInstruction().ToUniTask(cancellationToken: cancellationToken);
#else
            DOTween.Kill(animationId);
            activeTween = PlayTween();
            await activeTween.WithCancellation(cancellationToken: cancellationToken);
#endif
        }

        protected override void OnStop()
        {
#if PRIMETWEEN_INSTALLED
            activeTween.Stop();
#else
            DOTween.Kill(animationId);
#endif
        }

        protected override void OnSnap()
        {
            target.alpha = GetEndAlpha();
        }

#if PRIMETWEEN_INSTALLED
        private Tween PlayTween()
        {
            OnTweenEntered();

            var tween = Tween.Alpha(
                target,
                new TweenSettings<float>
                {
                    startValue = GetStartAlpha(),
                    endValue = GetEndAlpha(),
                    settings = new TweenSettings
                    {
                        ease = ease,
                        useUnscaledTime = isIndependentUpdate,
                        duration = Random.Range(duration.x, duration.y),
                    },
                }
            );

            tween.OnComplete(target: this, anim => anim.OnTweenExited());

            return tween;
        }
#else
        private Tween PlayTween()
        {
            OnTweenEntered();

            target.alpha = GetStartAlpha();

            return target
                .DOFade(GetEndAlpha(), Random.Range(duration.x, duration.y))
                .SetEase(ease)
                .SetId(animationId)
                .SetUpdate(isIndependentUpdate: isIndependentUpdate)
                .OnComplete(OnTweenExited);
        }
#endif

        private void OnTweenEntered()
        {
            OnPlayEntered?.Invoke();
            onPlayEntered.Invoke();
        }

        private void OnTweenExited()
        {
            OnPlayExited?.Invoke();
            onPlayExited.Invoke();

            if (isDestroyOnCompleted)
            {
                Destroy(this);
            }
        }

        private float GetStartAlpha()
        {
            return startAlphaMode switch
            {
                AlphaMode.FixedAlpha => startAlpha,
                AlphaMode.AlphaMultiplier => initialAlpha * startAlpha,
                AlphaMode.CurrentAlpha => target.alpha,
                AlphaMode.InitialAlpha => initialAlpha,
                _ => initialAlpha,
            };
        }

        private float GetEndAlpha()
        {
            return endAlphaMode switch
            {
                AlphaMode.FixedAlpha => endAlpha,
                AlphaMode.AlphaMultiplier => initialAlpha * endAlpha,
                AlphaMode.CurrentAlpha => target.alpha,
                AlphaMode.InitialAlpha => initialAlpha,
                _ => initialAlpha,
            };
        }

        private bool IsShowStartAlphaEditor()
        {
            return startAlphaMode is AlphaMode.FixedAlpha or AlphaMode.AlphaMultiplier;
        }

        private bool IsShowEndAlphaEditor()
        {
            return endAlphaMode is AlphaMode.FixedAlpha or AlphaMode.AlphaMultiplier;
        }
    }
}
