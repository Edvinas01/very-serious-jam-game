using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

#if PRIMETWEEN_INSTALLED
using PrimeTween;

#else
using DG.Tweening;
#endif

namespace InSun.GameCore.Animations
{
    internal sealed class RectAnchorAnimation : TweenAnimation
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private RectTransform target;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling", Expanded = true)]
#else
        [Header("Scaling")]
#endif
        [Tooltip("Tween from " + nameof(startAnchor) + " or initial anchor of " + nameof(target) + "?")]
        [SerializeField]
        private bool isTweenFromStartAnchor = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.ShowIf(nameof(isTweenFromStartAnchor))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTweenFromStartAnchor))]
#endif
        [SerializeField]
        private Vector2 startAnchor = Vector2.zero;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.PropertySpace]
#else
        [Space]
#endif
        [Tooltip("Tween to " + nameof(endAnchor) + " or initial anchor of " + nameof(target) + "?")]
        [SerializeField]
        private bool isTweenToEndAnchor;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Scaling")]
        [Sirenix.OdinInspector.ShowIf(nameof(isTweenToEndAnchor))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTweenToEndAnchor))]
#endif
        [SerializeField]
        private Vector2 endAnchor = Vector2.zero;

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
        private Vector2 initialAnchor;

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

        private void Reset()
        {
            target = (RectTransform)transform;
        }

        protected override void Awake()
        {
            base.Awake();

            if (target == false)
            {
                target = (RectTransform)transform;
            }

#if PRIMETWEEN_INSTALLED == false
            animationId = Guid.NewGuid().ToString();
#endif
        }

        protected override void Start()
        {
            base.Start();

            initialAnchor = target.anchoredPosition;

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
            target.anchoredPosition = isTweenToEndAnchor ? endAnchor : initialAnchor;
        }

#if PRIMETWEEN_INSTALLED
        private Tween PlayTween()
        {
            OnTweenEntered();

            var tween = Tween.UIAnchoredPosition(
                target,
                new TweenSettings<Vector2>
                {
                    startValue = isTweenFromStartAnchor ? startAnchor : target.anchoredPosition,
                    endValue = isTweenToEndAnchor ? endAnchor : initialAnchor,
                    settings = new TweenSettings
                    {
                        ease = ease,
                        useUnscaledTime = isIndependentUpdate,
                        duration = UnityEngine.Random.Range(duration.x, duration.y),
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

            target.anchoredPosition = isTweenFromStartAnchor ? startAnchor : target.anchoredPosition;

            return target
                .DOAnchorPos(isTweenToEndAnchor ? endAnchor : initialAnchor, UnityEngine.Random.Range(duration.x, duration.y))
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
    }
}
