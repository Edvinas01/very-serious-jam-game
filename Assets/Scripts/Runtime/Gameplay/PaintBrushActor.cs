using InSun.GameCore.Interactables;
using UnityEngine;
using UnityEngine.Serialization;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PaintBrushActor : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField]
        private SimpleInteractable interactable;

        [Header("Paint Tip")]
        [SerializeField]
        private Renderer paintTipRenderer;

        [FormerlySerializedAs("paintPoint")]
        [SerializeField]
        private Transform paintTip;

        [Header("Painting")]
        [SerializeField]
        private Color paintColor = Color.crimson;

        [Min(0)]
        [SerializeField]
        private int paintScore = 1;

        [Header("Raycasts")]
        [Min(0f)]
        [SerializeField]
        private float paintTriggerRadius = 0.1f;

        [Min(1)]
        [SerializeField]
        private int paintTexelRadius = 20;

        [SerializeField]
        private LayerMask paintLayerMask;

        private static readonly RaycastHit[] HitBuffer = new RaycastHit[10];
        private MaterialPropertyBlock paintTipPropertyBlock;

        private Vector3 originalPosition;
        private Quaternion originalRotation;

        public int PaintScore => paintScore;

        public int Radius => paintTexelRadius;

        public Color Color => paintColor;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (paintTipRenderer == false)
            {
                return;
            }

            SetColor(paintColor);
        }
#endif

        private void OnDrawGizmos()
        {
            if (paintTip == false)
            {
                return;
            }

            var gizmoColor = paintColor;
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(paintTip.position, paintTriggerRadius);

            gizmoColor.a = 0.4f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(paintTip.position, paintTriggerRadius);
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

        private void Awake()
        {
            paintTipPropertyBlock = new MaterialPropertyBlock();
            paintTipRenderer.GetPropertyBlock(paintTipPropertyBlock);

            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        private void Start()
        {
            SetColor(paintColor);
        }

        private void FixedUpdate()
        {
            var count = Physics.RaycastNonAlloc(
                origin: paintTip.position,
                direction: paintTip.forward,
                results: HitBuffer,
                maxDistance: paintTriggerRadius,
                layerMask: paintLayerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );

            for (var index = 0; index < count; index++)
            {
                var hit = HitBuffer[index];
                var paintable = hit.collider.GetComponentInParent<PaintableActor>();
                if (paintable == false)
                {
                    continue;
                }

                paintable.Paint(uv: hit.textureCoord, brush: this);
            }
        }

        private void OnHoverEntered(InteractableHoverEnteredArgs args)
        {
        }

        private void OnHoverExited(InteractableHoverExitedArgs args)
        {
        }

        private void OnInteractionEntered(InteractableInteractionEnteredArgs args)
        {
            if (args.Interactor.TryGetComponent(out HandActor handActor) == false)
            {
                return;
            }

            transform.SetParent(handActor.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void OnInteractionExited(InteractableInteractionExitedArgs args)
        {
            transform.SetParent(null);
            transform.position = originalPosition;
            transform.rotation = originalRotation;
        }

        private void SetColor(Color color)
        {
            paintTipPropertyBlock ??= new MaterialPropertyBlock();

            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            paintTipPropertyBlock.SetColor("_BaseColor", color);
            paintTipRenderer.SetPropertyBlock(paintTipPropertyBlock);
        }
    }
}
