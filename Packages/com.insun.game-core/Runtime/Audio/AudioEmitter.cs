using System.Collections.Generic;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    public sealed class AudioEmitter : MonoBehaviour
    {
        private enum StopMode
        {
            None = 0,
            OnDisable = 1,
            OnDestroy = 2,
        }

        [Header("General")]
        [Tooltip("Audio event to play (note, the event will be pooled according to its settings)")]
        [SerializeField]
        private AudioEventData audioEvent;

        [Tooltip("Used when for passing velocity to the underlying audio sources")]
        [SerializeField]
        private Rigidbody rigidBody;

        [Header("Features")]
        [SerializeField]
        private StopMode stopMode = StopMode.OnDisable;

        [Tooltip("Should emitter be followed by the audio source (useful for dynamic objects)?")]
        [SerializeField]
        private bool isFollowEmitter;

        [Tooltip("Should emitter automatically trigger on Start()")]
        [SerializeField]
        private bool isPlayOnStart;

        private readonly List<IAudioInstance> activeInstances = new();
        private IAudioSystem audioSystem;

        public Vector2 DistanceRange
        {
            get
            {
                if (audioEvent == false)
                {
                    return default;
                }

                return audioEvent.DistanceRange;
            }
        }

        public bool IsPlaying
        {
            get
            {
                foreach (var audioInstance in activeInstances)
                {
                    if (audioInstance.IsPlaying)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public AudioEventData AudioEvent
        {
            get => audioEvent;
            set => audioEvent = value;
        }

        public IReadOnlyList<IAudioInstance> ActiveInstances => activeInstances;

        private void OnDrawGizmosSelected()
        {
            if (audioEvent == false)
            {
                return;
            }

            var range = audioEvent.DistanceRange / 2f;

            {
                var color = Color.green;

                color.a = 1f;
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.position, range.x);

                color.a = 0.3f;
                Gizmos.color = color;
                Gizmos.DrawSphere(transform.position, range.x);
            }

            {
                var color = Color.yellowNice;

                color.a = 1f;
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.position, range.y);

                color.a = 0.2f;
                Gizmos.color = color;
                Gizmos.DrawSphere(transform.position, range.y);
            }
        }

        private void Awake()
        {
            if (rigidBody == false)
            {
                rigidBody = GetComponentInParent<Rigidbody>();
            }

            audioSystem = Game.GetObject<IAudioSystem>();
        }

        private void Start()
        {
            if (isPlayOnStart)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            if (stopMode == StopMode.OnDisable)
            {
                Stop();
            }
        }

        private void OnDestroy()
        {
            if (stopMode == StopMode.OnDestroy)
            {
                Stop();
            }
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (audioEvent == false || isActiveAndEnabled == false)
            {
                return;
            }

            var newInstance = audioSystem.Play(
                new PlayAudioEventArgs(audioEvent, owner: gameObject)
                {
                    IsFollowOwner = isFollowEmitter,
                    Position = transform.position,
                    RigidBody = rigidBody,
                }
            );

            if (newInstance.IsPlaying == false)
            {
                return;
            }

            activeInstances.Add(newInstance);

            newInstance.OnStopped += OnInstanceStopped;
        }

        public bool TryPlay(out IAudioInstance instance)
        {
            if (audioEvent == false || isActiveAndEnabled == false)
            {
                instance = null;
                return false;
            }

            var newInstance = audioSystem.Play(
                new PlayAudioEventArgs(audioEvent, owner: gameObject)
                {
                    IsFollowOwner = isFollowEmitter,
                    Position = transform.position,
                    RigidBody = rigidBody,
                }
            );

            if (newInstance.IsPlaying == false)
            {
                instance = null;
                return false;
            }

            activeInstances.Add(newInstance);

            newInstance.OnStopped += OnInstanceStopped;

            instance = newInstance;
            return true;
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            for (var index = activeInstances.Count - 1; index >= 0; index--)
            {
                var audioInstance = activeInstances[index];
                audioInstance.OnStopped -= OnInstanceStopped;
                audioInstance.Stop();
            }

            activeInstances.Clear();
        }

        private void OnInstanceStopped(IAudioInstance instance)
        {
            activeInstances.Remove(instance);
            instance.OnStopped -= OnInstanceStopped;
        }
    }
}
