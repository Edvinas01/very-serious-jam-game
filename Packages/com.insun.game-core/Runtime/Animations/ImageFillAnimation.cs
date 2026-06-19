using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

#if PRIMETWEEN_INSTALLED
using PrimeTween;

#else
using DG.Tweening;
#endif

namespace InSun.GameCore.Animations
{
    internal sealed class ImageFillAnimation : TweenAnimation
    {
        private enum FillMode
        {
            [Tooltip("Fill by using a fixed value to fill from/to")]
            FixedFill = 0,

            [Tooltip("Fill by multiplying the initial fill by given value")]
            FillMultiplier = 1,

            [Tooltip("Fill from/to current image fill")]
            CurrentFill = 2,

            [Tooltip("Fill from/to initial image fill")]
            InitialFill = 3,
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private Image target;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Fill", Expanded = true)]
#else
        [Header("Fill")]
#endif
        [SerializeField]
        private FillMode startFillMode = FillMode.FixedFill;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Fill")]
        [Sirenix.OdinInspector.PropertyRange(0f, 1f)]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowStartFillEditor))]
#else
        [Range(0f, 1f)]
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(IsShowStartFillEditor))]
#endif
        [SerializeField]
        private float startFill;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Fill")]
        [Sirenix.OdinInspector.PropertySpace]
#else
        [Space]
#endif
        [SerializeField]
        private FillMode endFillMode = FillMode.FixedFill;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Fill")]
        [Sirenix.OdinInspector.PropertyRange(0f, 1f)]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowEndFillEditor))]
#else
        [Range(0f, 1f)]
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(IsShowEndFillEditor))]
#endif
        [SerializeField]
        private float endFill = 1f;

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
        private float initialFill;

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
            target = GetComponent<Image>();
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            if (target == false)
            {
                target = GetComponent<Image>();
            }

#if PRIMETWEEN_INSTALLED == false
            animationId = Guid.NewGuid().ToString();
#endif
        }

        protected override void Start()
        {
            base.Start();

            initialFill = target.fillAmount;

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
            target.fillAmount = GetEndFill();
        }

#if PRIMETWEEN_INSTALLED
        private Tween PlayTween()
        {
            OnTweenEntered();

            var tween = Tween.UIFillAmount(
                target,
                new TweenSettings<float>
                {
                    startValue = GetStartFill(),
                    endValue = GetEndFill(),
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

            target.fillAmount = GetStartFill();

            return target
                .DOFillAmount(GetEndFill(), Random.Range(duration.x, duration.y))
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

        private float GetStartFill()
        {
            return startFillMode switch
            {
                FillMode.FixedFill => startFill,
                FillMode.FillMultiplier => initialFill * startFill,
                FillMode.CurrentFill => target.fillAmount,
                FillMode.InitialFill => initialFill,
                _ => initialFill,
            };
        }

        private float GetEndFill()
        {
            return endFillMode switch
            {
                FillMode.FixedFill => endFill,
                FillMode.FillMultiplier => initialFill * endFill,
                FillMode.CurrentFill => target.fillAmount,
                FillMode.InitialFill => initialFill,
                _ => initialFill,
            };
        }

        private bool IsShowStartFillEditor()
        {
            return startFillMode is FillMode.FixedFill or FillMode.FillMultiplier;
        }

        private bool IsShowEndFillEditor()
        {
            return endFillMode is FillMode.FixedFill or FillMode.FillMultiplier;
        }
    }
}
