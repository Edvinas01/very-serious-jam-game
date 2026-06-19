using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    internal sealed class PhysicalMovementController : MonoBehaviour
    {
        [Min(0f)]
        [SerializeField]
        private float maxSpeed = 5f;

        [Min(0f)]
        [SerializeField]
        private float acceleration = 20f;

        private Rigidbody rigidBody;

        public Transform ForwardTransform { get; set; }

        public Vector2 MoveAxis { get; set; }

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (ForwardTransform == false)
            {
                return;
            }

            var lookForward = Vector3.ProjectOnPlane(ForwardTransform.forward, Vector3.up).normalized;
            var lookRight = Vector3.ProjectOnPlane(ForwardTransform.right, Vector3.up).normalized;
            var moveDir = lookForward * MoveAxis.y + lookRight * MoveAxis.x;

            var horizontalVelocity = new Vector3(rigidBody.linearVelocity.x, 0f, rigidBody.linearVelocity.z);
            var speedDiff = moveDir * maxSpeed - horizontalVelocity;

            rigidBody.AddForce(speedDiff * acceleration, ForceMode.Acceleration);

            var targetRotation = Quaternion.Euler(0f, ForwardTransform.eulerAngles.y, 0f);
            rigidBody.MoveRotation(targetRotation);
        }
    }
}
