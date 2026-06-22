using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Crank")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Crank Data",
        fileName = "Data_Crank"
    )]
    internal sealed class CrankData : ScriptableObject
    {
        [SerializeField]
        private float maxTorque = 30f;

        [SerializeField]
        private float maxAngularSpeed = 20f;

        [SerializeField]
        private float inputSmoothing = 5f;

        [SerializeField]
        private float damping = 2f;

        [Min(0f)]
        [SerializeField]
        private float rotationDeltaMultiplier = 0.1f;

        public float MaxTorque => maxTorque;

        public float InputSmoothing => inputSmoothing;

        public float MaxAngularSpeed => maxAngularSpeed;

        public float Damping => damping;

        public float RotationDeltaMultiplier => rotationDeltaMultiplier;
    }
}
