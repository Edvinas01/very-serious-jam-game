using InSun.GameCore.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class CrankActor : MonoBehaviour
    {
        [SerializeField]
        private Transform handleTransform;

        [SerializeField]
        private Rigidbody crankRigidbody;

        [SerializeField]
        private SimpleInteractable interactable;

        [Header("Grabbing")]
        [SerializeField]
        private Transform anchorOffsetTransform;

        [Header("Torque")]
        [SerializeField]
        private float torqueMultiplier = 50f;

        [SerializeField]
        private float damping = 5f;

        [Min(0f)]
        [SerializeField]
        private float rotationDeltaMultiplier = 1f;

        private Rigidbody handTargetRigidbody;

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
            RotationDelta = crankRigidbody.angularVelocity.z * rotationDeltaMultiplier;

            if (handTargetRigidbody == null)
            {
                return;
            }

            var mouseDelta = Mouse.current.delta.ReadValue();
            crankRigidbody.AddTorque(Vector3.back * (mouseDelta.x * torqueMultiplier));
            crankRigidbody.angularVelocity *= 1f - damping * Time.fixedDeltaTime;
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
