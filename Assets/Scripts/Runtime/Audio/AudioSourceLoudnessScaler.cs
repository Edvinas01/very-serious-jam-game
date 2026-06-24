// See
// https://discussions.unity.com/t/how-do-i-get-the-current-volume-level-amplitude-of-playing-audio-not-the-set-volume-but-how-loud-it-is/162556

using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceLoudnessScaler : MonoBehaviour
    {
        [SerializeField]
        private Transform scaleTransform;

        [Min(0f)]
        [SerializeField]
        private float updateStep = 0.05f;

        [Min(0)]
        [SerializeField]
        private int sampleDataLength = 1024;

        [SerializeField]
        private Vector2 loudnessRange = new(0f, 0.1f);

        [Min(0f)]
        [SerializeField]
        private Vector3 maxScale = new(1.3f, 1.3f, 1.3f);

        [Min(0f)]
        [SerializeField]
        private float scaleLerpSpeed = 10f;

        private float currentUpdateTime;

        private Vector3 initialScale;
        private Vector3 currentScale;

        private AudioSource audioSource;
        private float[] clipSampleData;
        private float clipLoudness;

        public float NormalizedLoudness => Mathf.InverseLerp(loudnessRange.x, loudnessRange.y, clipLoudness);

        public float Loudness => clipLoudness;

        private void Awake()
        {
            if (scaleTransform == false)
            {
                scaleTransform = transform;
            }

            audioSource = GetComponent<AudioSource>();
            clipSampleData = new float[sampleDataLength];
        }

        private void Start()
        {
            initialScale = scaleTransform.localScale;
            currentScale = initialScale;
        }

        private void Update()
        {
            UpdateLoudness();
            UpdateScale();
        }

        private void UpdateLoudness()
        {
            if (sampleDataLength <= 0)
            {
                return;
            }

            var audioClip = audioSource.clip;
            if (audioClip == false)
            {
                return;
            }

            currentUpdateTime += Time.deltaTime;
            if (currentUpdateTime < updateStep)
            {
                return;
            }

            currentUpdateTime = 0f;

            var offset = Mathf.Clamp(audioSource.timeSamples, 0, audioClip.samples - sampleDataLength);
            audioClip.GetData(clipSampleData, offset);

            clipLoudness = 0f;

            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }

            clipLoudness /= sampleDataLength;
        }

        private void UpdateScale()
        {
            var targetScale = Vector3.Lerp(initialScale, maxScale, NormalizedLoudness);
            currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * scaleLerpSpeed);

            scaleTransform.localScale = currentScale;
        }
    }
}
