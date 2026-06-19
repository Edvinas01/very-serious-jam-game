using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableLong),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Long",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    public sealed class ScriptableLong : ScriptableVariable<long>
    {
        [SerializeField]
        private long baseValue;

        public override long BaseValue => baseValue;
    }
}

