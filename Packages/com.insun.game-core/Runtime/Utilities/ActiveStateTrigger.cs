using UnityEngine;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    internal sealed class ActiveStateTrigger : MonoBehaviour
    {
        [SerializeField]
        private GameObject target;

        [SerializeField]
        private UnityEvent onActive;

        [SerializeField]
        private UnityEvent onInactive;

        public void Trigger()
        {
            if (target.activeInHierarchy)
            {
                onActive.Invoke();
            }
            else
            {
                onInactive.Invoke();
            }
        }
    }
}
