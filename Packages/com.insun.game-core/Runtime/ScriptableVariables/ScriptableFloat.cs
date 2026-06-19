using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableFloat),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Float",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    public sealed class ScriptableFloat : ScriptableVariable<float>
    {
        [SerializeField]
        private float baseValue;

        public override float BaseValue => baseValue;
    }
}

