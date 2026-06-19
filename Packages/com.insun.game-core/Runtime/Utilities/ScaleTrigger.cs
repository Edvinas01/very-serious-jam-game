using UnityEngine;

namespace InSun.GameCore.Utilities
{
    internal sealed class ScaleTrigger : MonoBehaviour
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

        [Header("Scaling")]
        [SerializeField]
        private Transform target;

        [SerializeField]
        private Vector3 targetScale = Vector3.one;

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
            target.localScale = targetScale;
        }
    }
}
