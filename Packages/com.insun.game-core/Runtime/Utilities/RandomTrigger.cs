using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    internal sealed class RandomTrigger : MonoBehaviour
    {
        [Header("Features")]
        [Range(0f, 1f)]
        [SerializeField]
        private float triggerChance = 0.1f;

        [SerializeField]
        private bool isTriggerOnStart = true;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onTrigger;

        private void Start()
        {
            if (isTriggerOnStart == false)
            {
                return;
            }

            if (triggerChance <= 0 || Random.value < 1f - triggerChance)
            {
                return;
            }

            onTrigger.Invoke();
        }

        public void Trigger()
        {
            if (triggerChance <= 0 || Random.value < 1f - triggerChance)
            {
                return;
            }

            onTrigger.Invoke();
        }
    }
}
