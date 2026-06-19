#if FMOD_INSTALLED

using FMODUnity;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    internal sealed class FMODCollisionTrigger : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private StudioEventEmitter emitter;

        [Header("Features")]
        [Min(0f)]
        [SerializeField]
        private float minSpeed = 1f;

        [ParamRef]
        [SerializeField]
        private string collisionSpeedParameter = "ImpactSpeed";

        private void Reset()
        {
            emitter = GetComponent<StudioEventEmitter>();
        }

        private void Awake()
        {
            if (emitter == false)
            {
                emitter = GetComponent<StudioEventEmitter>();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            var speed = collision.relativeVelocity.magnitude;
            if (speed < minSpeed)
            {
                return;
            }

            emitter.Play();
            emitter.SetParameter(collisionSpeedParameter, speed);
        }
    }
}

#endif
