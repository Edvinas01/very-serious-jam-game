using UnityEngine;

namespace InSun.GameCore.Animations
{
    public sealed class WaveAnimator : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
#else
        [Header("General")]
#endif
        [SerializeField]
        private Transform target;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features", Expanded = true)]
        [Sirenix.OdinInspector.PropertyRange(0f, 100f)]
#else
        [Header("Features")]
        [Range(0f, 100f)]
#endif
        [SerializeField]
        private float amplitude = 0.1f;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Features")]
        [Sirenix.OdinInspector.PropertyRange(0f, 100f)]
#else
        [Range(0f, 100f)]
#endif
        [SerializeField]
        private float frequency = 2f;

        private Vector3 initialScale;
        private bool isPlaying;
        private float time;

        private void Awake()
        {
            initialScale = target.localScale;
        }

        private void Update()
        {
            if (isPlaying == false)
            {
                return;
            }

            time += Time.deltaTime;

            var scaleOffset = Mathf.Sin(time * frequency) * amplitude;
            target.localScale = initialScale * (1f + scaleOffset);
        }

        public void StartAnimation()
        {
            time = 0f;
            initialScale = target.localScale;
            isPlaying = true;
        }

        public void StopAnimation()
        {
            isPlaying = false;
            target.localScale = initialScale;
        }
    }
}
