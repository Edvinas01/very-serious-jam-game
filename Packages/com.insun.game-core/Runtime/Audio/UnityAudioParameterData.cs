using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/Audio/Unity Audio Parameter",
        fileName = MenuConstants.BaseAssetFileName + "Data_AudioParameter"
    )]
    internal sealed class UnityAudioParameterData : AudioParameterData
    {
        [SerializeField]
        private string parameterName;

        public override string Name => parameterName;
    }
}
