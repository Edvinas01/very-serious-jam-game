using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableInt),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Int",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    public sealed class ScriptableInt : ScriptableVariable<int>
    {
        [SerializeField]
        private int baseValue;

        public override int BaseValue => baseValue;
    }
}
