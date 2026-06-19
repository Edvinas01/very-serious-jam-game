#if PRIMETWEEN_INSTALLED == false

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace InSun.GameCore.Animations
{
    [Obsolete("Use " + nameof(ScaleAnimation) + " instead")]
    internal sealed class ScaleAnimationOld : TweenAnimation
    {
        [Header("General")]
        [SerializeField]
        private Transform target;

        [Header("Scaling")]
        [Tooltip("Tween from " + nameof(startScale) + " or initial scale of " + nameof(target) + "?")]
        [SerializeField]
        private bool isTweenFromStartScale = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isTweenFromStartScale))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTweenFromStartScale))]
#endif
        [SerializeField]
        private Vector3 startScale = Vector3.zero;

        [Min(0f)]
        [SerializeField]
        private float startScaleMultiplier = 1f;

        [Space]
        [Tooltip("Tween to " + nameof(endScale) + " or initial scale of " + nameof(target) + "?")]
        [SerializeField]
        private bool isTweenToEndScale;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isTweenToEndScale))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isTweenToEndScale))]
#endif
        [SerializeField]
        private Vector3 endScale = Vector3.one;

        [Header("Tween")]
        [SerializeField]
        private Ease ease = Ease.InOutCubic;

        [Range(0f, 60f)]
        [SerializeField]
        private float duration = 1f;

        [Range(0f, 60f)]
        [SerializeField]
        private float randomDurationOffset = 0.5f;

        [Header("Features")]
        [SerializeField]
        private bool isPlayOnStart = true;

        [SerializeField]
        private bool isDestroyOnCompleted = true;

        [SerializeField]
        private bool isIndependentUpdate = true;

        [SerializeField]
        private bool isPlayOnce;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onStarted;

        [SerializeField]
        private UnityEvent onCompleted;

        private Tween activeTween;
        private bool isTriggeredOnce;
        private Vector3 initialScale;
        private string animationId;

        public override bool IsPlaying
        {
            get
            {
                if (activeTween == null)
                {
                    return false;
                }

                if (activeTween.IsActive() == false)
                {
                    return false;
                }

                return activeTween.IsPlaying();
            }
        }

        public override Vector2 Duration
        {
            get => new(duration, randomDurationOffset);
            set
            {
                duration = value.x;
                randomDurationOffset = value.y;
            }
        }

        public override event Action OnPlayEntered;

        public override event Action OnPlayExited;

        private void Reset()
        {
            target = transform;
        }

        protected override void Awake()
        {
            base.Awake();

            if (target == false)
            {
                target = transform;
            }

            animationId = Guid.NewGuid().ToString();
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
            DOTween.Kill(animationId);

            PlayTween();
        }

        protected override async UniTask OnPlayAsync(CancellationToken cancellationToken = default)
        {
            if (isPlayOnce && isTriggeredOnce)
            {
                return;
            }

            isTriggeredOnce = true;
            DOTween.Kill(animationId);

            await PlayTween().WithCancellation(cancellationToken: cancellationToken);
        }

        protected override void OnStop()
        {
            DOTween.Kill(animationId);
        }

        protected override void OnSnap()
        {
            target.localScale = isTweenToEndScale ? endScale : initialScale;
        }

        private void OnTweenEntered()
        {
            OnPlayEntered?.Invoke();
            onStarted.Invoke();
        }

        private void OnTweenExited()
        {
            OnPlayExited?.Invoke();
            onCompleted.Invoke();

            if (isDestroyOnCompleted)
            {
                Destroy(this);
            }
        }

        private Tween PlayTween()
        {
            OnTweenEntered();

            target.localScale = isTweenFromStartScale ? startScale * startScaleMultiplier : target.localScale * startScaleMultiplier;

            activeTween = target
                .DOScale(isTweenToEndScale ? endScale : initialScale, duration + Random.Range(0f, randomDurationOffset))
                .SetEase(ease)
                .SetId(animationId)
                .SetUpdate(isIndependentUpdate: isIndependentUpdate)
                .OnComplete(OnTweenExited);

            return activeTween;
        }
    }
}
#endif
