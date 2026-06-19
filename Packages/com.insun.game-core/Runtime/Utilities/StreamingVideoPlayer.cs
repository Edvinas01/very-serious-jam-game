#if UNITY_VIDEO_INSTALLED

using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

namespace InSun.GameCore.Utilities
{
    internal sealed class StreamingVideoPlayer : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private VideoPlayer videoPlayer;

        [SerializeField]
        private string videoName;

        [SerializeField]
        private RawImage rawImage;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onVideoPrepared;

        [SerializeField]
        private UnityEvent onVideoEnded;

        private RenderTexture renderTexture;

        private void Start()
        {
            var path = Path.Combine(Application.streamingAssetsPath, videoName);
            videoPlayer.url = path;
            videoPlayer.Prepare();
            // videoPlayer.Play();
        }

        private void OnEnable()
        {
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.prepareCompleted += OnPrepareCompleted;
        }

        private void OnDisable()
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.prepareCompleted -= OnPrepareCompleted;
        }

        private void OnDestroy()
        {
            videoPlayer.targetTexture = null;
            rawImage.texture = null;

            if (renderTexture)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }
        }

        private void OnPrepareCompleted(VideoPlayer source)
        {
            InitializeRenderTexture(source);
            source.Play();
            onVideoPrepared.Invoke();
        }

        private void OnVideoEnd(VideoPlayer source)
        {
            onVideoEnded.Invoke();
        }

        private void InitializeRenderTexture(VideoPlayer source)
        {
            if (renderTexture)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            var newRenderTexture = new RenderTexture((int)source.width, (int)source.height, depth: 0);
            source.targetTexture = newRenderTexture;
            rawImage.texture = newRenderTexture;

            renderTexture = newRenderTexture;
        }
    }
}

#endif
