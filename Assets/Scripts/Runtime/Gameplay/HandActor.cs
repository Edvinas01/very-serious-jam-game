using System;
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

        [SerializeField]
        private Camera targetCamera;

        [Header("Smooth Damp")]
        [SerializeField]
        private float smoothTime = 0.05f;

        [Header("Interaction")]
        [SerializeField]
        private SimpleInteractor interactor;

        [Header("Input")]
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
        }

        private void OnEnable()
        {
            handPickUpAction.action.performed += OnPickUpPreformed;
            handPickUpAction.action.canceled += OnPickUpCancelled;

            handPushAction.action.performed += OnHandPushedPreformed;
            handPushAction.action.canceled += OnHandPushedCancelled;
        }

        private void OnDisable()
        {
            handPickUpAction.action.performed -= OnPickUpPreformed;
            handPickUpAction.action.canceled -= OnPickUpCancelled;

            handPushAction.action.performed -= OnHandPushedPreformed;
            handPushAction.action.canceled -= OnHandPushedCancelled;
        }

        private void Update()
        {
            MoveTarget();

            if (followTarget == null)
            {
                transform.position = Vector3.SmoothDamp(transform.position, targetRigidbody.position, ref smoothVelocity, smoothTime);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime / smoothTime);
                transform.rotation = followRotation;
            }
        }

        private void MoveTarget()
        {
            var mousePosition = Mouse.current.position.ReadValue();
            var distanceToPlane = originalDistance - targetCamera.transform.position.z;
            var worldPosition = targetCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distanceToPlane));

            if (isPushing)
            {
                worldPosition.z = originalDistance + pushDistance;
            }
            else
            {
                worldPosition.z = originalDistance;
            }

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

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
            originalParent = transform.parent;
            followRotation = transform.rotation;
            transform.SetParent(target);
        }

        public void ClearFollowTarget()
        {
            transform.SetParent(originalParent);
            transform.rotation = followRotation;
            followTarget = null;
            originalParent = null;
            smoothVelocity = Vector3.zero;
        }

        public Rigidbody GetArmJointRigidbody()
        {
            return targetRigidbody;
        }
    }
}
