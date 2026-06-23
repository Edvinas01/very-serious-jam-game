using Animancer;
using DoubleD.VerySeriousJamGame.Runtime.Audio;
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

        [FormerlySerializedAs("paintPoint")]
        [SerializeField]
        private Transform paintTip;

        [Header("Rendering")]
        [SerializeField]
        private Renderer brushRenderer;

        [Min(0)]
        [SerializeField]
        private int tipMaterialIndex = 1;

        [SerializeField]
        private Color paintColor = Color.crimson;

        [Header("Raycasts")]
        [Min(0f)]
        [SerializeField]
        private float paintTriggerRadius = 0.12f;

        [SerializeField]
        private LayerMask paintLayerMask;

        [Header("Score")]
        [Min(0)]
        [SerializeField]
        private int paintScore = 1;

        [Header("Painting")]
        [SerializeField]
        private ParticleSystem paintParticles;

        [Min(1)]
        [SerializeField]
        private int paintTexelRadius = 6;

        [SerializeField]
        private bool isSmoothEdges = true;

        [Min(0f)]
        [SerializeField]
        private float paintSmoothing = 10f;

        [Header("Audio")]
        [SerializeField]
        private AudioSource paintAudioSource;

        [SerializeField]
        private AnimationCurve paintValueVolumeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [SerializeField]
        private AudioData paintAudio;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onPaintEntered;

        [SerializeField]
        private UnityEvent onPaintExited;

        private static readonly RaycastHit[] HitBuffer = new RaycastHit[10];
        private MaterialPropertyBlock paintTipPropertyBlock;

        private float paintAmount;

        private Transform originalParent;
        private Vector3 originalLocalPosition;
        private Quaternion originalLocalRotation;

        public int PaintScore => paintScore;

        public int Radius => paintTexelRadius;

        public Color Color => paintColor;

        public bool IsPainting { get; private set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (brushRenderer == false)
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
            brushRenderer.GetPropertyBlock(paintTipPropertyBlock);

            originalParent = transform.parent;
            originalLocalPosition = transform.localPosition;
            originalLocalRotation = transform.localRotation;
        }

        private void Start()
        {
            SetColor(paintColor);
        }

        private void FixedUpdate()
        {
            var isPaintingPrev = IsPainting;

            var count = Physics.RaycastNonAlloc(
                origin: paintTip.position,
                direction: paintTip.forward,
                results: HitBuffer,
                maxDistance: paintTriggerRadius,
                layerMask: paintLayerMask,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
            );

            var isRaycastPainted = false;
            for (var index = 0; index < count; index++)
            {
                var hit = HitBuffer[index];
                var paintable = hit.collider.GetComponentInParent<PaintableActor>();
                if (paintable == false)
                {
                    continue;
                }

                var isPainted = paintable.TryPaint(uv: hit.textureCoord, brush: this, isSmoothEdges: isSmoothEdges);
                if (isPainted && paintParticles)
                {
                    var emitParams = new ParticleSystem.EmitParams
                    {
                        position = hit.point,
                        applyShapeToPosition = true,
                        startColor = paintColor,
                    };

                    paintParticles.Emit(emitParams, 1);
                }

                if (isPainted)
                {
                    isRaycastPainted = true;
                }
            }

            var paintTarget = isRaycastPainted ? 1f : 0f;
            paintAmount = Mathf.Lerp(paintAmount, paintTarget, paintSmoothing * Time.fixedDeltaTime);
            IsPainting = paintAmount >= 0.5f;

            if (paintAudioSource)
            {
                var audioVolume = paintValueVolumeCurve.Evaluate(paintAmount);
                if (audioVolume > 0f)
                {
                    paintAudioSource.volume = audioVolume;

                    if (paintAudioSource.isPlaying == false)
                    {
                        paintAudioSource.PlayUsing(paintAudio);
                    }
                }
                else
                {
                    paintAudioSource.Stop();
                }
            }

            UpdatePaintingEvents(isPaintingPrev, IsPainting);
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
            transform.SetParent(originalParent);
            transform.localPosition = originalLocalPosition;
            transform.localRotation = originalLocalRotation;
        }

        private void SetColor(Color color)
        {
            paintTipPropertyBlock ??= new MaterialPropertyBlock();

            // ReSharper disable once Unity.PreferAddressByIdToGraphicsParams
            paintTipPropertyBlock.SetColor("_BaseColor", color);
            brushRenderer.SetPropertyBlock(paintTipPropertyBlock, tipMaterialIndex);
        }

        private void UpdatePaintingEvents(bool isPaintingPrev, bool isPaintingNext)
        {
            if (isPaintingPrev == isPaintingNext)
            {
                return;
            }

            if (isPaintingNext)
            {
                onPaintEntered.Invoke();
            }
            else
            {
                onPaintExited.Invoke();
            }
        }
    }
}
