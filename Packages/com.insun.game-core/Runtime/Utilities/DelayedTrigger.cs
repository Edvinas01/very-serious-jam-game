using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    internal sealed class DelayedTrigger : MonoBehaviour
    {
        private enum TriggerEvent
        {
            None = 0,
            Start = 1,
            Awake = 2,
        }

        [Header("Triggering")]
        [SerializeField]
        private TriggerEvent autoTrigger = TriggerEvent.Start;

        [SerializeField]
        private Vector2 delayRange = new(0.1f, 1f);

        [Header("Events")]
        [SerializeField]
        private UnityEvent onTriggered;

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

        public void Trigger()
        {
            TriggerAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public async UniTask TriggerAsync(CancellationToken cancellationToken)
        {
            var delay = delayRange.GetRandomFloat();
            if (delay > 0f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: cancellationToken);
            }

            OnTriggered?.Invoke();
            onTriggered.Invoke();
        }
    }
}
