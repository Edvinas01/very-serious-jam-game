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
        }

        private void ReadMoveInput()
        {
            mouseMove = handMoveAction.action.ReadValue<Vector2>();
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
