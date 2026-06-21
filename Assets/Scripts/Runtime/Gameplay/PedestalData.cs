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

        public float ConstantSpeed => constantSpeed;

        public float SpinDecaySpeed => spinDecaySpeed;
    }
}
