using DoubleD.VerySeriousJamGame.Runtime.Audio;
using InSun.GameCore.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class HandActor : MonoBehaviour
    {
        [Header("Hand Visuals")]
        [SerializeField]
        private Renderer quadRenderer;

        [SerializeField]
        private Texture2D openHandTexture;

        [SerializeField]
        private Texture2D closedHandTexture;

        [Header("Movement")]
        [SerializeField]
        private float pushDistance = 1f;

        [Min(0f)]
        [SerializeField]
        private float pullBackDistance;

        [SerializeField]
        private float toolLength;

        [SerializeField]
        private float noToolLength;

        [Min(0f)]
        [SerializeField]
        private float pushCastRadius = 0.05f;

        [SerializeField]
        private LayerMask pushLayerMask;

        [SerializeField]
        private Camera targetCamera;

        [Header("Smooth Damp")]
        [SerializeField]
        private float smoothTime = 0.05f;

        [Header("Interaction")]
        [SerializeField]
        private SimpleInteractor interactor;

        [Header("Tilt")]
        [SerializeField]
        private float maxTiltX = 30f;

        [SerializeField]
        private float maxTiltY = 30f;

        [SerializeField]
        private float tiltStep = 0.5f;

        [SerializeField]
        private float tiltSmoothTime = 0.1f;

        [SerializeField]
        private float paintTiltStep = 5f;

        [SerializeField]
        private PedestalActor pedestal;

        [Header("Audio")]
        [SerializeField]
        private AudioSource handAudioSource;

        [SerializeField]
        private AudioData grabAudio;

        [SerializeField]
        private AudioData dropAudio;

        [Header("Input")]
        [SerializeField]
        private InputActionReference pointerAction;

        [SerializeField]
        private InputActionReference handPickUpAction;

        [SerializeField]
        private InputActionReference handPushAction;

        private MaterialPropertyBlock propertyBlock;
        private Rigidbody targetRigidbody;

        private float originalDistance;
        private bool isPushing;
        private Transform followTarget;
        private Transform originalParent;
        private Quaternion followRotation;
        private Vector3 smoothVelocity;
        private bool skipMoveTarget;

        private Vector2 previousMousePosition;
        private float currentTiltX;
        private float currentTiltY;
        private float tiltVelocityX;
        private float tiltVelocityY;
        private PaintBrushActor heldBrush;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            originalDistance = transform.position.z;

            CreateHandJoint();
        }

        private void Start()
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            propertyBlock.SetTexture("_BaseMap", openHandTexture);
            quadRenderer.SetPropertyBlock(propertyBlock);

            previousMousePosition = pointerAction.action.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            handPickUpAction.action.performed += OnPickUpPreformed;
            handPickUpAction.action.canceled += OnPickUpCancelled;

            handPushAction.action.performed += OnHandPushedPreformed;
            handPushAction.action.canceled += OnHandPushedCancelled;

            interactor.OnInteractionEntered += OnInteractionEntered;
            interactor.OnInteractionExited += OnInteractionExited;
        }

        private void OnDisable()
        {
            handPickUpAction.action.performed -= OnPickUpPreformed;
            handPickUpAction.action.canceled -= OnPickUpCancelled;

            handPushAction.action.performed -= OnHandPushedPreformed;
            handPushAction.action.canceled -= OnHandPushedCancelled;

            interactor.OnInteractionEntered -= OnInteractionEntered;
            interactor.OnInteractionExited -= OnInteractionExited;
        }

        private void Update()
        {
            MoveTarget();
            UpdateTilt();

            if (followTarget == null)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetRigidbody.position, ref smoothVelocity, smoothTime);
                transform.rotation = Quaternion.Euler(currentTiltX, currentTiltY, 0f);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime / smoothTime);
                transform.rotation = followRotation;
            }
        }

        private void UpdateTilt()
        {
            var mousePosition = pointerAction.action.ReadValue<Vector2>();
            var delta = mousePosition - previousMousePosition;
            previousMousePosition = mousePosition;

            if (interactor.IsInteracting)
            {
                if (heldBrush == null)
                {
                    heldBrush = GetComponentInChildren<PaintBrushActor>();
                }

                var surfaceTiltY = 0f;

                if (pedestal && heldBrush && heldBrush.IsPainting)
                {
                    surfaceTiltY = -pedestal.SpinSpeed * paintTiltStep;
                }

                var targetTiltX = Mathf.Clamp(delta.y * tiltStep, -maxTiltX, maxTiltX);
                var targetTiltY = Mathf.Clamp(-delta.x * tiltStep + surfaceTiltY, -maxTiltY, maxTiltY);

                currentTiltX = Mathf.SmoothDamp(currentTiltX, targetTiltX, ref tiltVelocityX, tiltSmoothTime);
                currentTiltY = Mathf.SmoothDamp(currentTiltY, targetTiltY, ref tiltVelocityY, tiltSmoothTime);
            }
            else
            {
                heldBrush = null;
                currentTiltX = Mathf.SmoothDamp(currentTiltX, 0f, ref tiltVelocityX, tiltSmoothTime);
                currentTiltY = Mathf.SmoothDamp(currentTiltY, 0f, ref tiltVelocityY, tiltSmoothTime);
            }
        }

        private void MoveTarget()
        {
            if (skipMoveTarget)
            {
                skipMoveTarget = false;
                return;
            }

            var pointerPosition = pointerAction.action.ReadValue<Vector2>();
            var distanceToPlane = originalDistance - targetCamera.transform.position.z;
            var worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(pointerPosition.x, pointerPosition.y, distanceToPlane));

            var activeToolLength = heldBrush != null ? toolLength : noToolLength;
            var castOrigin = new Vector3(worldPosition.x, worldPosition.y, originalDistance - pullBackDistance);

            var maxPush = isPushing ? pushDistance - activeToolLength : 0f;
            var fullArmDistance = pullBackDistance + pushDistance + activeToolLength;

            var hits = Physics.SphereCastAll(castOrigin, pushCastRadius, Vector3.forward, fullArmDistance, pushLayerMask);
            foreach (var hit in hits)
            {
                if (hit.collider.transform.IsChildOf(transform))
                {
                    continue;
                }

                var pushCandidate = hit.distance - pullBackDistance - activeToolLength;
                if (pushCandidate < maxPush)
                {
                    maxPush = pushCandidate;
                }
            }

            worldPosition.z = originalDistance + maxPush;

            targetRigidbody.MovePosition(worldPosition);
        }


        private void CreateHandJoint()
        {
            var handJointTarget = new GameObject("HandJointTarget");
            handJointTarget.transform.SetParent(transform.parent);
            handJointTarget.transform.position = transform.position;

            targetRigidbody = handJointTarget.AddComponent<Rigidbody>();
            targetRigidbody.isKinematic = true;
        }


        private void OnPickUpPreformed(InputAction.CallbackContext context)
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            propertyBlock.SetTexture("_BaseMap", closedHandTexture);
            quadRenderer.SetPropertyBlock(propertyBlock);

            interactor.StartInteraction();
        }

        private void OnPickUpCancelled(InputAction.CallbackContext context)
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            propertyBlock.SetTexture("_BaseMap", openHandTexture);
            quadRenderer.SetPropertyBlock(propertyBlock);

            interactor.StopInteraction();
        }

        private void OnHandPushedPreformed(InputAction.CallbackContext context)
        {
            isPushing = true;
        }

        private void OnHandPushedCancelled(InputAction.CallbackContext context)
        {
            isPushing = false;
        }

        private void OnInteractionEntered(IInteractable interactable)
        {
            handAudioSource.PlayUsing(grabAudio);
        }

        private void OnInteractionExited(IInteractable interactable)
        {
            handAudioSource.PlayUsing(dropAudio);
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            originalParent = transform.parent;
            followRotation = transform.rotation;
            transform.SetParent(target);
        }

        public void ClearFollowTarget()
        {
            var handWorldPos = transform.position;

            transform.SetParent(originalParent);
            transform.rotation = followRotation;

            followTarget = null;
            originalParent = null;
            smoothVelocity = Vector3.zero;

            targetRigidbody.position = handWorldPos;
            transform.position = handWorldPos;

            skipMoveTarget = true;

            var screenPos = targetCamera.WorldToScreenPoint(handWorldPos);
            Mouse.current.WarpCursorPosition(new Vector2(screenPos.x, screenPos.y));
            previousMousePosition = new Vector2(screenPos.x, screenPos.y);
        }

        public Rigidbody GetArmJointRigidbody()
        {
            return targetRigidbody;
        }
    }
}
