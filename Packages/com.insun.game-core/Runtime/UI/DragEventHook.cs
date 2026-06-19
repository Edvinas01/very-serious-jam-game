using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace InSun.GameCore.UI
{
    internal sealed class DragEventHook : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onDragEntered;

        [SerializeField]
        private UnityEvent onDragExited;

        [FormerlySerializedAs("onValidDragEntered")]
        [SerializeField]
        private UnityEvent onValidZoneEntered;

        [FormerlySerializedAs("onValidDragExited")]
        [SerializeField]
        private UnityEvent onValidZoneExited;

        public bool IsDragging
        {
            set
            {
                if (value)
                {
                    onDragEntered.Invoke();
                }
                else
                {
                    onDragExited.Invoke();
                }
            }
        }

        public bool IsValidZone
        {
            set
            {
                if (value)
                {
                    onValidZoneEntered.Invoke();
                }
                else
                {
                    onValidZoneExited.Invoke();
                }
            }
        }
    }
}
