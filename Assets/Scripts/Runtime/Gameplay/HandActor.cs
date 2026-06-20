using System;
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
        private float moveSpeed = 5f;

        [SerializeField]
        private Vector2 maxMoveDistance = new (10f, 10f);

        [Header("Input")]
        [SerializeField]
        private InputActionReference handMoveAction;

        [SerializeField]
        private InputActionReference handPickUpAction;

        private MaterialPropertyBlock propertyBlock;
        private Vector2 mouseMove;
        private bool isPickingUp;

        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
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
        }

        private void OnDisable()
        {
            handPickUpAction.action.performed -= OnPickUpPreformed;
            handPickUpAction.action.canceled -= OnPickUpCancelled;
        }

        private void Update()
        {
            ReadMoveInput();
            MoveHand();
        }

        private void ReadMoveInput()
        {
            mouseMove = handMoveAction.action.ReadValue<Vector2>();
        }

        private void MoveHand()
        {
            var position = transform.localPosition;
            position.x += mouseMove.x * moveSpeed * Time.deltaTime;
            position.y += mouseMove.y * moveSpeed * Time.deltaTime;

            position.x = Mathf.Clamp(position.x, -maxMoveDistance.x, maxMoveDistance.x);
            position.y = Mathf.Clamp(position.y, -maxMoveDistance.y, maxMoveDistance.y);

            transform.localPosition = position;
        }

        private void OnPickUpPreformed(InputAction.CallbackContext context)
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            propertyBlock.SetTexture("_BaseMap", closedHandTexture);
            quadRenderer.SetPropertyBlock(propertyBlock);
        }

        private void OnPickUpCancelled(InputAction.CallbackContext context)
        {
            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            propertyBlock.SetTexture("_BaseMap", openHandTexture);
            quadRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
