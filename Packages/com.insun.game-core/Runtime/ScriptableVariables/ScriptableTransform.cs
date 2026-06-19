using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [CreateAssetMenu(
        fileName = CreateAssetMenuConstants.BaseFileName + nameof(ScriptableTransform),
        menuName = CreateAssetMenuConstants.BaseMenuName + "/Scriptable Transform",
        order = CreateAssetMenuConstants.BaseOrder
    )]
    internal sealed class ScriptableTransform : ScriptableVariable<Transform>
    {
    }
}
