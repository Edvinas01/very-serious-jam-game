using InSun.GameCore;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class PedestalObjectActor : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer targetMesh;

        [Min(0)]
        [SerializeField]
        private int painMaskWidth = 1024;

        [Min(0)]
        [SerializeField]
        private int painMaskHeight = 1024;

        private int paintMaskPropertyId;
        private MaterialPropertyBlock propertyBlock;

        private Texture2D paintMaskTexture;
        private bool isPaintedThisFrame;

        private void Awake()
        {
            // Init share stuffs
            paintMaskPropertyId = Shader.PropertyToID("_PaintMask");
            propertyBlock = new MaterialPropertyBlock();

            // Create mask texture
            // var meshMaterial = targetMesh.sharedMaterial;
            // var originalTex = meshMaterial.mainTexture as Texture2D;
            // var width = originalTex ? originalTex.width : 1024;
            // var height = originalTex ? originalTex.height : 1024;

            paintMaskTexture = new Texture2D(
                width: painMaskWidth,
                height: painMaskHeight,
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

            // Apply mask to renderer
            propertyBlock.SetTexture(paintMaskPropertyId, paintMaskTexture);
            targetMesh.SetPropertyBlock(propertyBlock);
        }


        private void OnEnable()
        {
            Game.AddObject<int, PedestalObjectActor>(GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Game.RemoveObject<int, PedestalObjectActor>(GetInstanceID());
        }

        private void LateUpdate()
        {
            if (isPaintedThisFrame == false)
            {
                return;
            }

            paintMaskTexture.Apply();
            isPaintedThisFrame = false;
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
