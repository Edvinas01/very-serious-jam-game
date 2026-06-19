using UnityEngine;

namespace InSun.GameCore.Pooling
{
    [RequireComponent(typeof(IPooledObject))]
    [RequireComponent(typeof(ParticleSystem))]
    internal sealed class PooledParticles : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem particles;

        private IPooledObject pooledObject;

        public bool IsPlaying => particles.isPlaying;

#if UNITY_EDITOR
        private void Reset()
        {
            particles = GetComponent<ParticleSystem>();
        }
#endif

        private void Awake()
        {
            pooledObject = GetComponent<IPooledObject>();

            if (particles == false)
            {
                particles = GetComponent<ParticleSystem>();
            }

            var mainModule = particles.main;
            mainModule.playOnAwake = false;
            mainModule.stopAction = ParticleSystemStopAction.Disable;

            pooledObject.OnRetrieved += OnPooledObjectRetrieved;
            pooledObject.OnReleased += OnPooledObjectReleased;
        }

        private void OnDestroy()
        {
            pooledObject.OnRetrieved -= OnPooledObjectRetrieved;
            pooledObject.OnReleased -= OnPooledObjectReleased;
        }

        private void OnPooledObjectRetrieved(IPooledObject obj)
        {
            particles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.Play(withChildren: true);
        }

        private void OnPooledObjectReleased(IPooledObject obj)
        {
            particles.Stop(withChildren: true);
        }
    }
}
