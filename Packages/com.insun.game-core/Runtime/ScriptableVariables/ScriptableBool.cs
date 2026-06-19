using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableBool),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Bool",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    public sealed class ScriptableBool : ScriptableVariable<bool>
    {
        [SerializeField]
        private bool baseValue;

        public override bool BaseValue => baseValue;
    }
}

