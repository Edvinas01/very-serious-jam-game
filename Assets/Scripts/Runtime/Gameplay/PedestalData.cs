using DoubleD.VerySeriousJamGame.Runtime.Audio;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Pedestal")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Pedestal Data",
        fileName = "Data_Pedestal"
    )]
    internal sealed class PedestalData : ScriptableObject
    {
        [SerializeField]
        private float constantSpeed;

        [Min(0f)]
        [SerializeField]
        private float spinDecaySpeed = 0.4f;

        [SerializeField]
        private float maxSpinSpeed = 360f;

        [Header("Audio")]
        [SerializeField]
        private AudioData spinAudio;

        [SerializeField]
        private AnimationCurve spinPitchCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 2f);

        public float ConstantSpeed => constantSpeed;

        public float SpinDecaySpeed => spinDecaySpeed;

        public float MaxSpinSpeed => maxSpinSpeed;

        public AudioData SpinAudio => spinAudio;

        public AnimationCurve SpinPitchCurve => spinPitchCurve;
    }
}
