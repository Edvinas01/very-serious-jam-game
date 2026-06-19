using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Utilities
{
    [RequireComponent(typeof(SpriteRenderer))]
    internal sealed class ParallaxController : MonoBehaviour
    {
        [Header("Parallax")]
        [SerializeField]
        [Range(0f, 1f)]
        private float parallaxX = 0.5f;

        [SerializeField]
        [Range(0f, 1f)]
        private float parallaxY = 0.5f;

        [Header("Movement")]
        [SerializeField]
        private float moveSpeedX;

        [SerializeField]
        private float moveSpeedY;

        [Header("Repeat")]
        [SerializeField]
        private bool isRepeatX;

        [SerializeField]
        private bool isRepeatY;

        [Min(0)]
        [SerializeField]
        private int repeatBuffer;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isRepeatX))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isRepeatX))]
#endif
        [SerializeField]
        private float repeatSpacingX;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isRepeatY))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isRepeatY))]
#endif
        [SerializeField]
        private float repeatSpacingY;

        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;

        private Vector3 initialPosition;

        private Vector2 moveOffset;

        private Vector2 repeatSize;
        private Vector2 repeatStep;
        private Vector2 repeatSpan;

        private readonly List<Transform> parallaxTransforms = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying == false || spriteRenderer == false || mainCamera == false)
            {
                return;
            }

            foreach (var parallaxTransform in parallaxTransforms)
            {
                DestroyImmediate(parallaxTransform.gameObject);
            }

            parallaxTransforms.Clear();

            if (isRepeatX || isRepeatY)
            {
                InitializeRepeat();
            }
        }
#endif

        private void Awake()
        {
            if (transform.parent && transform.parent.GetComponent<ParallaxController>())
            {
                // We're copying parallax objects, need to destroy controller so it doesn't recurse
                Destroy(this);
                enabled = false;
                return;
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;

            if (mainCamera == false)
            {
                Debug.LogError("Cannot parallax, no camera in the scene", this);
                enabled = false;
                return;
            }

            initialPosition = transform.position;
        }

        private void Start()
        {
            if (isRepeatX || isRepeatY)
            {
                InitializeRepeat();
            }
        }

        private void LateUpdate()
        {
            var cameraPosition = mainCamera.transform.position;

            var deltaTime = Time.deltaTime;
            moveOffset.x += moveSpeedX * deltaTime;
            moveOffset.y += moveSpeedY * deltaTime;

            var targetPosition = transform.position;
            targetPosition.x = initialPosition.x + cameraPosition.x * parallaxX + moveOffset.x;
            targetPosition.y = initialPosition.y + cameraPosition.y * parallaxY + moveOffset.y;

            transform.position = targetPosition;

            if (parallaxTransforms.Count > 0)
            {
                UpdateRepeat();
            }
        }

        private void InitializeRepeat()
        {
            repeatSize = spriteRenderer.bounds.size;
            repeatStep = new Vector2(repeatSize.x + repeatSpacingX, repeatSize.y + repeatSpacingY);

            var cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
            var cameraHalfHeight = mainCamera.orthographicSize;

            var rangeX = isRepeatX ? Mathf.CeilToInt(cameraHalfWidth / repeatStep.x) + repeatBuffer : 0;
            var rangeY = isRepeatY ? Mathf.CeilToInt(cameraHalfHeight / repeatStep.y) + repeatBuffer : 0;

            repeatSpan.x = (2 * rangeX + 1) * repeatStep.x;
            repeatSpan.y = (2 * rangeY + 1) * repeatStep.y;

            // Can't just copy as it will re-copy the hierarchy
            var gameObjectTemplate = Instantiate(gameObject);
            gameObjectTemplate.SetActive(false);

            for (var x = -rangeX; x <= rangeX; x++)
            {
                for (var y = -rangeY; y <= rangeY; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    var offset = new Vector3(x * repeatStep.x, y * repeatStep.y, 0f);
                    var copy = CreateCopy(gameObjectTemplate, transform.position + offset);

                    parallaxTransforms.Add(copy.transform);
                }
            }

            Destroy(gameObjectTemplate);
        }

        private void UpdateRepeat()
        {
            if (mainCamera.orthographic == false)
            {
                return;
            }

            var cameraX = mainCamera.transform.position.x;
            var cameraY = mainCamera.transform.position.y;

            var halfSpanX = repeatSpan.x * 0.5f;
            var halfSpanY = repeatSpan.y * 0.5f;

            foreach (var parallaxTransform in parallaxTransforms)
            {
                var parallaxPosition = parallaxTransform.position;

                if (isRepeatX)
                {
                    var positionX = parallaxPosition.x - cameraX;

                    while (positionX > halfSpanX)
                    {
                        parallaxPosition.x -= repeatSpan.x;
                        positionX -= repeatSpan.x;
                    }

                    while (positionX < -halfSpanX)
                    {
                        parallaxPosition.x += repeatSpan.x;
                        positionX += repeatSpan.x;
                    }
                }

                if (isRepeatY)
                {
                    var positionY = parallaxPosition.y - cameraY;

                    while (positionY > halfSpanY)
                    {
                        parallaxPosition.y -= repeatSpan.y;
                        positionY -= repeatSpan.y;
                    }

                    while (positionY < -halfSpanY)
                    {
                        parallaxPosition.y += repeatSpan.y;
                        positionY += repeatSpan.y;
                    }
                }

                parallaxTransform.position = parallaxPosition;
            }
        }

        private GameObject CreateCopy(GameObject template, Vector3 position)
        {
            var instance = Instantiate(template, position, transform.rotation, transform);
            instance.name = gameObject.name;
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);

            return instance;
        }
    }
}
