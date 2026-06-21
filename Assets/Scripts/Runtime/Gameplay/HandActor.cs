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
        private Rigidbody handRigidBody;

        [SerializeField]
        private Camera targetCamera;

        [Header("Spring")]
        [SerializeField]
        private float springForce = 50f;

        [SerializeField]
        private float springDamper = 5f;

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

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
            originalDistance = transform.position.z;

            SetupJoint();
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

        private void SetupJoint()
        {
            var handJointTarget = new GameObject("HandJointTarget");
            handJointTarget.transform.SetParent(transform.parent);
            handJointTarget.transform.position = transform.position;

            targetRigidbody = handJointTarget.AddComponent<Rigidbody>();
            targetRigidbody.isKinematic = true;

            var joint = gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = targetRigidbody;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = Vector3.zero;

            var drive = new JointDrive
            {
                positionSpring = springForce,
                positionDamper = springDamper,
                maximumForce = float.MaxValue
            };

            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;

            joint.xMotion = ConfigurableJointMotion.Free;
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
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
    }
}
