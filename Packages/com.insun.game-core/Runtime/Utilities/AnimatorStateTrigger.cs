using UnityEngine;

namespace InSun.GameCore.Utilities
{
    internal sealed class AnimatorStateTrigger : MonoBehaviour
    {
        [SerializeField]
        private Animator targetAnimator;

        [SerializeField]
        private string stateName = "Player In";

        [SerializeField]
        [Range(0f, 1f)]
        private float transitionTime = 0.3f;

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

        public void SetState()
        {
            targetAnimator.CrossFade(stateName, transitionTime);
        }
    }
}
