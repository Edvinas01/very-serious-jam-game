using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    internal sealed class LoopTrigger : MonoBehaviour
    {
        private enum TriggerEvent
        {
            None = 0,
            Start = 1,
            Awake = 2,
        }

        [Header("Triggering")]
        [SerializeField]
        private TriggerEvent autoTrigger = TriggerEvent.None;

        [Min(1)]
        [SerializeField]
        private Vector2Int loopCount = new(1, 10);

        [Min(0f)]
        [SerializeField]
        private Vector2 loopDelay = new(0, 0);

        [Header("Events")]
        [SerializeField]
        private UnityEvent onTriggered;

        private CancellationTokenSource cancellationTokenSource;

        public event Action OnTriggered;

        private void Awake()
        {
            if (autoTrigger == TriggerEvent.Awake)
            {
                Trigger();
            }
        }

        private void Start()
        {
            if (autoTrigger == TriggerEvent.Start)
            {
                Trigger();
            }
        }

        private void OnDisable()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        public void Trigger()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            TriggerAsync(cancellationTokenSource.Token).Forget();
        }

        public async UniTask TriggerAsync(CancellationToken cancellationToken)
        {
            var count = loopCount.GetRandomInt();
            for (var index = 0; index < count; index++)
            {
                var delay = loopDelay.GetRandomFloat();
                if (delay > 0f)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
                }

                OnTriggered?.Invoke();
                onTriggered.Invoke();
            }
        }
    }
}
