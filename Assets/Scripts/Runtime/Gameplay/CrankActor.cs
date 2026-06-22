using InSun.GameCore.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class CrankActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private CrankData data;

        [Header("Interaction")]
        [SerializeField]
        private Transform handleTransform;

        [SerializeField]
        private Rigidbody crankRigidbody;

        [SerializeField]
        private SimpleInteractable interactable;

        [Header("Grabbing")]
        [SerializeField]
        private Transform anchorOffsetTransform;

        private Rigidbody handTargetRigidbody;
        private float smoothedInput;

        public float RotationDelta { get; private set; }

        private void OnEnable()
        {
            interactable.OnHoverEntered += OnHoverEntered;
            interactable.OnHoverExited += OnHoverExited;

            interactable.OnInteractionEntered += OnInteractionEntered;
            interactable.OnInteractionExited += OnInteractionExited;
        }

        private void OnDisable()
        {
            interactable.OnHoverEntered -= OnHoverEntered;
            interactable.OnHoverExited -= OnHoverExited;

            interactable.OnInteractionEntered -= OnInteractionEntered;
            interactable.OnInteractionExited -= OnInteractionExited;
        }

        private void FixedUpdate()
        {
            RotationDelta = crankRigidbody.angularVelocity.z * data.RotationDeltaMultiplier;

            crankRigidbody.angularVelocity *= 1f - data.Damping * Time.fixedDeltaTime;

            var turnVelocity = crankRigidbody.angularVelocity;
            turnVelocity.z = Mathf.Clamp(turnVelocity.z, -data.MaxAngularSpeed, data.MaxAngularSpeed);
            crankRigidbody.angularVelocity = turnVelocity;

            var target = 0f;
            if (handTargetRigidbody != null)
            {
                var mouseDelta = Mouse.current.delta.ReadValue();
                if (Mathf.Abs(mouseDelta.x) > 0.1f)
                {
                    target = Mathf.Sign(mouseDelta.x);
                }
            }

            smoothedInput = Mathf.Lerp(smoothedInput, target, data.InputSmoothing * Time.fixedDeltaTime);

            if (Mathf.Abs(smoothedInput) > 0.01f)
            {
                crankRigidbody.AddTorque(Vector3.back * (smoothedInput * data.MaxTorque));
            }
        }

        private void OnInteractionEntered(InteractableInteractionEnteredArgs args)
        {
            if (args.Interactor.TryGetComponent<HandActor>(out var handActor) == false)
            {
                return;
            }

            handActor.SetFollowTarget(handleTransform);
            handTargetRigidbody = handActor.GetArmJointRigidbody();
        }

        private void OnInteractionExited(InteractableInteractionExitedArgs args)
        {
            if (args.Interactor.TryGetComponent<HandActor>(out var handActor))
            {
                handActor.ClearFollowTarget();
            }

            handTargetRigidbody = null;
        }

        private void OnHoverExited(InteractableHoverExitedArgs args)
        {
        }

        private void OnHoverEntered(InteractableHoverEnteredArgs args)
        {
        }
    }
}
