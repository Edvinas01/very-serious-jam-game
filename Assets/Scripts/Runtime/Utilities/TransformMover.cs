using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Utilities
{
    internal sealed class TransformMover : MonoBehaviour
    {
        [SerializeField]
        private Transform moveTarget;

        private Vector3 initialWorldPosition;
        private Vector3 targetWorldPosition;

        private void Awake()
        {
            initialWorldPosition = transform.position;
        }

        private void FixedUpdate()
        {
            var newPosition = Vector3.Lerp(
                transform.position,
                targetWorldPosition,
                Time.deltaTime
            );

            transform.position = newPosition;
        }

        public void MoveToTarget()
        {
            targetWorldPosition = moveTarget.position;
        }

        public void MoveBack()
        {
            targetWorldPosition = initialWorldPosition;
        }
    }
}
