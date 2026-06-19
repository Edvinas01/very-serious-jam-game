using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    [ExecuteAlways]
    internal sealed class ScriptableTransformHook : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
        [Sirenix.OdinInspector.Required]
#endif
        [SerializeField]
        private ScriptableTransform scriptableTransform;

        private void OnEnable()
        {
            if (scriptableTransform == false)
            {
                enabled = false;
                return;
            }

            scriptableTransform.Value = transform;
        }

        private void OnDisable()
        {
            if (scriptableTransform)
            {
                scriptableTransform.Value = null;
            }
        }
    }
}
