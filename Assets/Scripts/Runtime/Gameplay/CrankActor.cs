using DoubleD.VerySeriousJamGame.Runtime.Audio;
using DoubleD.VerySeriousJamGame.Runtime.Utilities;
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

        [SerializeField]
        private TransformMover asideMover;

        [Header("Grabbing")]
        [SerializeField]
        private Transform anchorOffsetTransform;

        [Header("Audio")]
        [SerializeField]
        private AudioSource spinAudioSource;

        private Rigidbody handTargetRigidbody;
        private float smoothedInput;

        [Header("Runtime")]
        [SerializeField]
        private float delta;

        public float RotationDelta
        {
            get => delta;
            private set => delta = value;
        }

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

            UpdateSpinAudio(Mathf.Abs(RotationDelta));
        }

        public void MoveAside()
        {
            asideMover.MoveToTarget();
        }

        public void MoveBack()
        {
            asideMover.MoveBack();
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

        private void UpdateSpinAudio(float rotationDelta)
        {
            if (spinAudioSource == false || data.SpinAudio == null)
            {
                return;
            }

            var pitchValue = data.RotationDeltaPitchCurve.Evaluate(rotationDelta);
            if (pitchValue <= 0f)
            {
                spinAudioSource.Stop();
                return;
            }

            if (spinAudioSource.isPlaying == false)
            {
                spinAudioSource.PlayUsing(data.SpinAudio);
            }

            spinAudioSource.pitch = pitchValue;
        }
    }
}
