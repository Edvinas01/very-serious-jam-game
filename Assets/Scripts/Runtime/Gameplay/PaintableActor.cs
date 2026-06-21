using System;
using System.Threading;
using Animancer;
using Cysharp.Threading.Tasks;
using InSun.GameCore.Animations;
using UnityEngine;
using UnityEngine.Serialization;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class PaintableActor : MonoBehaviour
    {
        [FormerlySerializedAs("objectRenderer")]
        [Header("General")]
        [SerializeField]
        private PaintableData data;

        [Header("Body")]
        [SerializeField]
        private Transform bodyTransform;

        [SerializeField]
        private MeshRenderer bodyRenderer;

        [SerializeField]
        private MeshFilter bodyMeshFilter;

        [SerializeField]
        private MeshCollider bodyMeshCollider;

        [SerializeField]
        private Rigidbody rigidBody;

        [Header("Mask")]
        [Min(0)]
        [SerializeField]
        private int painMaskDefaultWidth = 256;

        [Min(0)]
        [SerializeField]
        private int painMaskDefaultHeight = 256;

        [Header("Animations")]
        [SerializeField]
        private AnimancerComponent animancer;

        [SerializeField]
        private TweenAnimation slidedInTween;

        private int paintMaskPropertyId;
        private MaterialPropertyBlock propertyBlock;

        private Texture2D paintMaskTexture;
        private bool[] paintedMask;
        private int paintedPixelCount;
        private bool isPaintedThisFrame;

        public PaintableData Data => data;

        public float PaintAmount { get; private set; }

        public bool IsKinematic
        {
            set
            {
                if (value == false)
                {
                    foreach (var meshCollider in GetComponentsInChildren<MeshCollider>())
                    {
                        meshCollider.convex = true;
                    }
                }

                rigidBody.isKinematic = value;
            }
        }

        public event Action<float> OnPainted;

        private void LateUpdate()
        {
            if (isPaintedThisFrame == false)
            {
                return;
            }

            paintMaskTexture.Apply();
            isPaintedThisFrame = false;

            var rawPercent = (float)paintedPixelCount / paintedMask.Length;
            var percent = Mathf.InverseLerp(Data.FullyPaintedRange.x, Data.FullyPaintedRange.y, rawPercent);

            PaintAmount = percent;

            OnPainted?.Invoke(percent);
        }

        [ContextMenu("Initialize")]
        private void Initialize()
        {
            if (data == false)
            {
                Debug.LogError($"{nameof(data)} is not set", this);
                return;
            }

            Initialize(data);
        }

        public void Initialize(PaintableData newData)
        {
            if (newData == false)
            {
                Debug.LogError("Trying to initialize with null data", this);
                return;
            }

            data = newData;

            name = $"{nameof(PaintableActor)} ({data.Name})";

            paintMaskPropertyId = Shader.PropertyToID("_PaintMask");
            propertyBlock = new MaterialPropertyBlock();

            // Apply mesh data
            bodyMeshFilter.sharedMesh = data.SharedMesh;
            bodyMeshCollider.sharedMesh = data.SharedMesh;
            bodyRenderer.sharedMaterial = data.Material;
            bodyTransform.localScale = data.Scale;
            bodyTransform.localRotation = Quaternion.Euler(data.Rotation);
            bodyTransform.localPosition = data.Offset;

            // Create paint mask texture
            var width = data.Texture ? data.Texture.width : painMaskDefaultWidth;
            var height = data.Texture ? data.Texture.height : painMaskDefaultHeight;

            paintMaskTexture = new Texture2D(
                width: width,
                height: height,
                textureFormat: TextureFormat.RGBA32,
                mipChain: false
            );

            // Clear mask texture
            var pixelData = paintMaskTexture.GetRawTextureData<Color32>();
            for (var index = 0; index < pixelData.Length; index++)
            {
                var initialColor = Color.white;
                initialColor.a = 0f;
                pixelData[index] = initialColor;
            }

            paintMaskTexture.Apply();

            paintedMask = new bool[width * height];

            // Apply mask to renderer
            propertyBlock.SetTexture(paintMaskPropertyId, paintMaskTexture);
            bodyRenderer.SetPropertyBlock(propertyBlock);
        }

        public async UniTask SlideInAsync(CancellationToken cancellationToken)
        {
            var clip = Data.SlideInClip;
            clip.SampleAnimation(gameObject, 0f);

            gameObject.SetActive(true);

            await animancer.Play(clip).ToUniTask(cancellationToken: cancellationToken);
            await slidedInTween.PlayAsync(cancellationToken);
        }

        public async UniTask SlideOutAsync(CancellationToken cancellationToken)
        {
            await animancer.Play(Data.SlideOutClip).ToUniTask(cancellationToken: cancellationToken);
        }

        public void Paint(Vector2 uv, int radius, Color color)
        {
            var pixelData = paintMaskTexture.GetRawTextureData<Color32>();
            var targetColor = (Color32)color;

            var textureW = paintMaskTexture.width;
            var textureH = paintMaskTexture.height;
            var textureX = Mathf.RoundToInt(uv.x * textureW);
            var textureY = Mathf.RoundToInt(uv.y * textureH);

            var radiusSqr = radius * radius;

            for (var dy = -radius; dy <= radius; dy++)
            {
                for (var dx = -radius; dx <= radius; dx++)
                {
                    var distanceSqr = dx * dx + dy * dy;
                    if (distanceSqr > radiusSqr)
                    {
                        continue;
                    }

                    var pixelX = textureX + dx;
                    var pixelY = textureY + dy;

                    if (pixelX < 0 || pixelX >= textureW || pixelY < 0 || pixelY >= textureH)
                    {
                        continue;
                    }

                    var pixelIndex = pixelY * textureW + pixelX;

                    if (paintedMask[pixelIndex] == false)
                    {
                        paintedMask[pixelIndex] = true;
                        paintedPixelCount++;
                    }

                    // Apply some fade on edges
                    var currentColor = pixelData[pixelIndex];
                    var colorFade = Mathf.InverseLerp(0, radiusSqr, distanceSqr);

                    pixelData[pixelIndex] = Color.Lerp(targetColor, currentColor, colorFade);
                }
            }

            isPaintedThisFrame = true;
        }
    }
}
