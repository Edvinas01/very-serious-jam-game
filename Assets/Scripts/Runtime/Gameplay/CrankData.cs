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
        private float torqueMultiplier = 2f;

        [SerializeField]
        private float damping = 7f;

        [Min(0f)]
        [SerializeField]
        private float rotationDeltaMultiplier = 0.1f;

        public float TorqueMultiplier => torqueMultiplier;

        public float Damping => damping;

        public float RotationDeltaMultiplier => rotationDeltaMultiplier;
    }
}
