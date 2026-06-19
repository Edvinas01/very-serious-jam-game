using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

#if PRIMETWEEN_INSTALLED
using PrimeTween;

#else
using DG.Tweening;
#endif


namespace InSun.GameCore.Animations
{
    internal sealed class ScaleAnimation : TweenAnimation
    {
        private enum ScaleMode
        {
            [Tooltip("Scale by using a fixed scale to scale from/to")]
            FixedScale = 0,

            [Tooltip("Scale by multiplying the initial scale by given values")]
            ScaleMultiplier = 1,

            [Tooltip("Scale from/to current object scale")]
            CurrentScale = 2,

            [Tooltip("Scale from/to initial object scale")]
            InitialScale = 3,
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private Transform target;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling", Expanded = true)]
#else
        [Header("Scaling")]
#endif
        [SerializeField]
        private ScaleMode startScaleMode = ScaleMode.FixedScale;


#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowStartScaleEditor))]
#else
        [SunnyInspector.ShowIf(nameof(IsShowStartScaleEditor))]
#endif
        [SerializeField]
        private Vector3 startScale = Vector3.zero;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.PropertySpace]
#else
        [Space]
#endif
        [SerializeField]
        private ScaleMode endScaleMode = ScaleMode.FixedScale;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.MinValue(0f)]
        [Sirenix.OdinInspector.ShowIf(nameof(IsShowEndScaleEditor))]
#else
        [Min(0f)]
        [SunnyInspector.ShowIf(nameof(IsShowEndScaleEditor))]
#endif
        [SerializeField]
        private Vector3 endScale = Vector3.one;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Tween", Expanded = true)]
#else
        [Header("Tween")]
#endif
        [SerializeField]
#if PRIMETWEEN_INSTALLED
        private Ease ease = Ease.InOutCubic;
#else
        private Ease ease = Ease.InOutCubic;
#endif

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
        [SerializeField]
        private bool isTriggerUnityEvents = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.ShowIf(nameof(isTriggerUnityEvents))]
#else
        [SunnyInspector.ShowIf(nameof(isTriggerUnityEvents))]
#endif
        [SerializeField]
        private UnityEvent onPlayEntered;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Events")]
        [Sirenix.OdinInspector.ShowIf(nameof(isTriggerUnityEvents))]
#else
        [SunnyInspector.ShowIf(nameof(isTriggerUnityEvents))]
#endif
        [SerializeField]
        private UnityEvent onPlayExited;

#if PRIMETWEEN_INSTALLED
        private Tween activeTween;
#else
        private Tween activeTween;
#endif

        private bool isTriggeredOnce;
        private Vector3 initialScale;

#if PRIMETWEEN_INSTALLED == false
        private string animationId = string.Empty;
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


                if (TweenExtensions.IsActive(activeTween) == false)
                {
                    return false;
                }

                return TweenExtensions.IsPlaying(activeTween);
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
            target = transform;
        }
#endif

        protected override void Awake()
        {
            base.Awake();

            if (target == false)
            {
                target = transform;
            }

#if PRIMETWEEN_INSTALLED == false
            animationId = Guid.NewGuid().ToString();
#endif
        }

        protected override void Start()
        {
            base.Start();

            initialScale = target.localScale;

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
            activeTween = PlayTween(ease);
#else
            DOTween.Kill(animationId);
            activeTween = PlayTween(ease);
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
            activeTween = PlayTween(ease);
            await activeTween.ToYieldInstruction().ToUniTask(cancellationToken: cancellationToken);
#else
            DOTween.Kill(animationId);
            activeTween = PlayTween(ease);
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
            target.localScale = GetEndScale();
        }

        private void OnTweenEntered()
        {
            OnPlayEntered?.Invoke();

            if (isTriggerUnityEvents)
            {
                onPlayEntered.Invoke();
            }
        }

        private void OnTweenExited()
        {
            OnPlayExited?.Invoke();

            if (isTriggerUnityEvents)
            {
                onPlayExited.Invoke();
            }

            if (isDestroyOnCompleted)
            {
                Destroy(this);
            }
        }

#if PRIMETWEEN_INSTALLED
        private Tween PlayTween(Ease tweenEase)
        {
            OnTweenEntered();

            var tween = Tween.Scale(
                target,
                new TweenSettings<Vector3>
                {
                    startValue = GetStartScale(),
                    endValue = GetEndScale(),
                    settings = new TweenSettings
                    {
                        ease = tweenEase,
                        useUnscaledTime = isIndependentUpdate,
                        duration = Random.Range(duration.x, duration.y),
                    },
                }
            );

            tween.OnComplete(target: this, anim => anim.OnTweenExited());

            return tween;
        }
#else
        private Tween PlayTween(Ease tweenEase)
        {
            OnTweenEntered();

            target.localScale = GetStartScale();

            return target
                .DOScale(GetEndScale(), Random.Range(duration.x, duration.y))
                .SetEase(tweenEase)
                .SetId(animationId)
                .SetUpdate(isIndependentUpdate: isIndependentUpdate)
                .OnComplete(OnTweenExited);
        }
#endif

        private Vector3 GetStartScale()
        {
            return startScaleMode switch
            {
                ScaleMode.FixedScale => startScale,
                ScaleMode.ScaleMultiplier => new Vector3(
                    startScale.x * initialScale.x,
                    startScale.y * initialScale.y,
                    startScale.z * initialScale.z
                ),
                ScaleMode.CurrentScale => target.localScale,
                ScaleMode.InitialScale => initialScale,
                _ => initialScale,
            };
        }

        private Vector3 GetEndScale()
        {
            return endScaleMode switch
            {
                ScaleMode.FixedScale => endScale,
                ScaleMode.ScaleMultiplier => new Vector3(
                    endScale.x * initialScale.x,
                    endScale.y * initialScale.y,
                    endScale.z * initialScale.z
                ),
                ScaleMode.CurrentScale => target.localScale,
                ScaleMode.InitialScale => initialScale,
                _ => initialScale,
            };
        }

        private bool IsShowStartScaleEditor()
        {
            return startScaleMode is ScaleMode.FixedScale or ScaleMode.ScaleMultiplier;
        }

        private bool IsShowEndScaleEditor()
        {
            return endScaleMode is ScaleMode.FixedScale or ScaleMode.ScaleMultiplier;
        }
    }
}
