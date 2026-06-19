using System;
using InSun.GameCore.SunnyInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace InSun.GameCore.Culling
{
    internal sealed class SimpleCullable : Cullable, ICullable
    {
        [Header("General")]
        [FormerlySerializedAs("offset")]
        [SerializeField]
        private Vector3 positionOffset;

        [Header("Visibility")]
        [Min(0f)]
        [SerializeField]
        private float visibilityRadius = 1f;

        [Header("Distance")]
        [SerializeField]
        private bool isDistanceCull;

        [Min(0f)]
        [SerializeField]
        [ShowIf(nameof(isDistanceCull))]
        private float maxDistance = 100f;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onBecameVisible;

        [SerializeField]
        private UnityEvent onBecameHidden;

        private ICullingReference cullingReference;
        private bool isInitialized;
        private bool isVisible;

        private CullingGroup cullingGroup;
        private BoundingSphere[] boundingSpheres;

        public override bool IsVisible => isVisible;

        public override float Radius
        {
            set
            {
                visibilityRadius = value;

                if (isInitialized)
                {
                    boundingSpheres[0].radius = value;
                }
            }
        }

        public override Transform ReferenceTransform
        {
            set
            {
                if (isInitialized)
                {
                    cullingGroup.SetDistanceReferencePoint(value);
                }
            }
        }

        public override Vector3 ReferencePosition
        {
            set
            {
                if (isInitialized)
                {
                    cullingGroup.SetDistanceReferencePoint(value);
                }
            }
        }

        public override Vector3 Position
        {
            set
            {
                if (isInitialized)
                {
                    boundingSpheres[0].position = value + positionOffset;
                }
            }
        }

        public override event Action OnCullableBecameVisible;

        public override event Action OnCullableBecameHidden;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (isInitialized)
            {
                boundingSpheres[0].radius = visibilityRadius;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            if (Application.isPlaying && boundingSpheres != null)
            {
                Gizmos.color = IsVisible ? Color.green : Color.crimson;
                Gizmos.DrawWireSphere(boundingSpheres[0].position + positionOffset, visibilityRadius);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position + positionOffset, visibilityRadius);
            }
        }
#endif

        private void Awake()
        {
            cullingReference = GetComponent<ICullingReference>();
            InitializeCulling();
        }

        private void OnEnable()
        {
            cullingGroup.targetCamera = cullingReference != null ? cullingReference.ReferenceCamera : Camera.main;
            boundingSpheres[0].position = transform.position + positionOffset;
            cullingGroup.onStateChanged += OnCullingStateChanged;
            cullingGroup.enabled = true;
            isVisible = cullingGroup.IsVisible(0);
        }

        private void OnDisable()
        {
            cullingGroup.onStateChanged -= OnCullingStateChanged;
            cullingGroup.enabled = false;
            isVisible = false;
        }

        private void OnDestroy()
        {
            DisposeCulling();
        }

        private void OnCullingStateChanged(CullingGroupEvent evt)
        {
            if (evt.hasBecomeVisible)
            {
                isVisible = true;
                OnCullableBecameVisible?.Invoke();
                onBecameVisible.Invoke();
            }

            if (evt.hasBecomeInvisible)
            {
                isVisible = false;
                OnCullableBecameHidden?.Invoke();
                onBecameHidden.Invoke();
            }
        }

        private void InitializeCulling()
        {
            boundingSpheres = new[]
            {
                new BoundingSphere(transform.position + positionOffset, visibilityRadius),
            };

            cullingGroup = new CullingGroup();
            cullingGroup.targetCamera = cullingReference != null ? cullingReference.ReferenceCamera : Camera.main;
            cullingGroup.SetBoundingSpheres(boundingSpheres);
            cullingGroup.SetBoundingSphereCount(1);

            if (isDistanceCull)
            {
                cullingGroup.SetBoundingDistances(new[] { 0f, maxDistance });

                if (cullingReference != null)
                {
                    cullingGroup.SetDistanceReferencePoint(cullingReference.ReferenceTransform);
                }
            }

            isInitialized = true;
        }

        private void DisposeCulling()
        {
            cullingGroup.Dispose();
            cullingGroup = null;

            isInitialized = false;
        }
    }
}
