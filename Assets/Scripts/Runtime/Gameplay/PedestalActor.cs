using DoubleD.VerySeriousJamGame.Runtime.Audio;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PedestalActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private PedestalData data;

        [Header("Paintables")]
        [SerializeField]
        private Transform objectParent;

        [Header("Audio")]
        [SerializeField]
        private AudioSource spinAudioSource;

        [Header("Runtime")]
        [SerializeField]
        private float currentSpinSpeed;

        public float SpinSpeed => data.ConstantSpeed + currentSpinSpeed;

        public Transform ObjectParent => objectParent;

        private void FixedUpdate()
        {
            var totalSpeed = data.ConstantSpeed + currentSpinSpeed;
            var deltaTime = Time.deltaTime;

            objectParent.Rotate(Vector3.up, totalSpeed * deltaTime, Space.World);
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0f, data.SpinDecaySpeed * deltaTime);

            UpdateSpinAudio(Mathf.Abs(totalSpeed));
        }

        public void AddSpinSpeed(float speed)
        {
            currentSpinSpeed = Mathf.Clamp(currentSpinSpeed + speed, -data.MaxSpinSpeed, data.MaxSpinSpeed);
        }

        private void UpdateSpinAudio(float absSpeed)
        {
            if (spinAudioSource == false || data.SpinAudio == null)
            {
                return;
            }

            var volumeValue = data.SpinVolumeCurve.Evaluate(absSpeed);
            var pitchValue = data.SpinPitchCurve.Evaluate(absSpeed);
            if (volumeValue <= 0f)
            {
                spinAudioSource.Stop();
                return;
            }

            if (spinAudioSource.isPlaying == false)
            {
                spinAudioSource.PlayUsing(data.SpinAudio);
            }

            spinAudioSource.volume = volumeValue;
            spinAudioSource.pitch = pitchValue;
        }
    }
}
