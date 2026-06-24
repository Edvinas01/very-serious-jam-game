using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Audio
{
    internal sealed class PedestalSpeedAudioModulator : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private PedestalActor pedestalActor;

        [SerializeField]
        private AudioSource audioSource;

        [Header("Remapping")]
        [SerializeField]
        private AnimationCurve pitchCurve;

        [SerializeField]
        private AnimationCurve volumeCurve;

        private void Awake()
        {
            if (pedestalActor == false)
            {
                pedestalActor = FindFirstObjectByType<PedestalActor>();
            }
        }

        private void Update()
        {
            var speed = Mathf.Abs(pedestalActor.SpinSpeed);

            if (pitchCurve != null && pitchCurve.length > 0)
            {
                audioSource.pitch = pitchCurve.Evaluate(speed);
            }

            if (volumeCurve != null && volumeCurve.length > 0)
            {
                audioSource.volume = volumeCurve.Evaluate(speed);
            }
        }
    }
}
