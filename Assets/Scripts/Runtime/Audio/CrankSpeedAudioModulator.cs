using DoubleD.VerySeriousJamGame.Runtime.Gameplay;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Audio
{
    internal sealed class CrankSpeedAudioModulator : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private CrankActor crankActor;

        [SerializeField]
        private AudioSource audioSource;

        [Header("Remapping")]
        [SerializeField]
        private AnimationCurve pitchCurve;

        [SerializeField]
        private AnimationCurve volumeCurve;

        private void Awake()
        {
            if (crankActor == false)
            {
                crankActor = FindFirstObjectByType<CrankActor>();
            }
        }

        private void Update()
        {
            var speed = Mathf.Abs(crankActor.RotationDelta);

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
