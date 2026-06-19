using UnityEngine;

namespace InSun.GameCore.Utilities
{
    internal sealed class AnimatorBoolTrigger : MonoBehaviour
    {
        [SerializeField]
        private Animator targetAnimator;

        [SerializeField]
        private string parameterName = "Player In";

        private void Awake()
        {
            if (targetAnimator == false)
            {
                targetAnimator = GetComponentInParent<Animator>();
            }

            if (targetAnimator == false)
            {
                Debug.LogError($"{nameof(targetAnimator)} is not set", this);
                enabled = false;
            }
        }

        public void EnableParameter()
        {
            targetAnimator.SetBool(parameterName, true);
        }

        public void DisableParameter()
        {
            targetAnimator.SetBool(parameterName, false);
        }
    }
}
