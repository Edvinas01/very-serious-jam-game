using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableDouble),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Double",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    public sealed class ScriptableDouble : ScriptableVariable<double>
    {
        [SerializeField]
        private double baseValue;

        public override double BaseValue => baseValue;
    }
}

