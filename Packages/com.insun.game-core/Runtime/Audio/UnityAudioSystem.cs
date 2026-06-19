#if UNITY_AUDIO_INSTALLED

using System.Collections.Generic;
using InSun.GameCore.Objects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace InSun.GameCore.Audio
{
    public sealed class UnityAudioSystem : MonoBehaviour, IAudioSystem, ILifecycleListener, IUpdateListener
    {
        [Header("General")]
        [SerializeField]
        private AudioMixer audioMixer;

        [Header("Snapshots")]
        [SerializeField]
        private AudioMixerSnapshot defaultSnapshot;

        [Header("Pooling")]
        [SerializeField]
        private AudioSourceExtended audioSourcePrefab;

        [SerializeField]
        private Transform parentTransform;

        private readonly Dictionary<IAudioEvent, ObjectPool<AudioSourceExtended>> audioPoolsByEvents = new();
        private readonly Dictionary<IAudioEvent, List<AudioSourceExtended>> activeSourcesByEvent = new();

        private readonly List<AudioSourceExtended> audioSources = new();
        private readonly List<AudioMixerSnapshot> snapshotStack = new();

        public bool IsLoading => false;

        public void OnInitialized()
        {
            if (defaultSnapshot)
            {
                snapshotStack.Add(defaultSnapshot);
            }
        }

        public void OnDisposed()
        {
        }

        public void Load()
        {
        }

        public void UnLoad()
        {
        }

        public float GetParameter(GetAudioParameterArgs args)
        {
            var audioParameter = args.AudioParameter;
            if (audioParameter is not UnityAudioParameterData unityData)
            {
                Debug.LogWarning($"Cannot get audio parameter {audioParameter.Name}, unsupported type", this);
                return 0f;
            }

            if (audioMixer.GetFloat(unityData.Name, out var valueDb) == false)
            {
                return 0f;
            }

            var valueLinear = valueDb <= -80f ? 0f : Mathf.Pow(10f, valueDb / 20f);

            return valueLinear;
        }

        public void SetParameter(SetAudioParameterArgs args)
        {
            var audioParameter = args.AudioParameter;
            if (audioParameter is not UnityAudioParameterData unityData)
            {
                Debug.LogWarning($"Cannot set audio parameter {audioParameter.Name}, unsupported type", this);
                return;
            }

            var valueLinear = args.Value;
            var valueDb = valueLinear <= 0f ? -80f : Mathf.Log10(valueLinear) * 20f;

            audioMixer.SetFloat(unityData.Name, valueDb);
        }

        public IAudioInstance Play(PlayAudioEventArgs args)
        {
            var audioEvent = args.AudioEvent;
            if (audioEvent is not UnityAudioEventData unityAudioEvent)
            {
                Debug.LogWarning($"Cannot play audio event {audioEvent.Name}, unsupported type", this);
                return DefaultAudioInstance.Instance;
            }

            if (unityAudioEvent.PlayChance > 0f && Random.value < 1f - unityAudioEvent.PlayChance)
            {
                return DefaultAudioInstance.Instance;
            }

            if (audioPoolsByEvents.TryGetValue(unityAudioEvent, out var pool) == false)
            {
                pool = CreatePool(unityAudioEvent);
                audioPoolsByEvents.Add(unityAudioEvent, pool);
            }

            if (activeSourcesByEvent.TryGetValue(unityAudioEvent, out var activeSources) && activeSources.Count >= unityAudioEvent.MaxInstanceCount)
            {
                if (unityAudioEvent.IsStealing == false)
                {
                    return DefaultAudioInstance.Instance;
                }

                var stolen = activeSources[0];
                stolen.Pool = pool;
                ConfigureSource(stolen, args, unityAudioEvent);

                stolen.IsPlayOnTop = true;
                stolen.Play();
                stolen.IsPlayOnTop = false;

                activeSources.RemoveAt(0);
                activeSources.Add(stolen);

                return stolen;
            }

            var source = pool.Get();
            source.Pool = pool;
            ConfigureSource(source, args, unityAudioEvent);

            source.Play();

            return source;
        }

        public void OnUpdated(float deltaTime)
        {
            for (var index = audioSources.Count - 1; index >= 0; index--)
            {
                var source = audioSources[index];
                if (source)
                {
                    source.Tick(deltaTime);
                }
                else
                {
                    audioSources.RemoveAt(index);
                }
            }
        }

        public void AddActiveAudioSource(AudioSourceExtended source)
        {
            audioSources.Add(source);
        }

        public void RemoveActiveAudioSource(AudioSourceExtended source)
        {
            audioSources.Remove(source);
        }

        public void DeactivateSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
        {
            if (snapshot != defaultSnapshot)
            {
                snapshotStack.Remove(snapshot);
            }

            var target = snapshotStack.Count > 0 ? snapshotStack[snapshotStack.Count - 1] : defaultSnapshot;
            target.TransitionTo(transitionTime);
        }

        public void ActivateSnapshot(AudioMixerSnapshot snapshot, float transitionTime)
        {
            var firstSnapshot = snapshotStack.Count > 0 ? snapshotStack[snapshotStack.Count - 1] : null;
            if (firstSnapshot == snapshot)
            {
                return;
            }

            snapshot.TransitionTo(transitionTime);
            snapshotStack.Add(snapshot);
        }

        private static void ConfigureSource(
            AudioSourceExtended source,
            PlayAudioEventArgs args,
            UnityAudioEventData unityAudioEvent
        )
        {
            source.transform.position = args.Position;
            source.FollowTransform = args.IsFollowOwner ? args.Owner.transform : null;

            source.AudioRolloffMode = unityAudioEvent.AudioRolloffMode;
            if (unityAudioEvent.AudioRolloffMode == AudioRolloffMode.Custom)
            {
                source.SetCurve(AudioSourceCurveType.CustomRolloff, unityAudioEvent.CustomRolloffCurve);
            }

            source.IsLoop = unityAudioEvent.IsLoop;
            source.IsPlayOnTop = false;
            source.MixerGroup = unityAudioEvent.MixerGroup;
            source.RandomClips = unityAudioEvent.AudioClips;

            source.IsFadeInOnPlay = unityAudioEvent.IsFadeInOnPlay;
            source.FadeInDuration = unityAudioEvent.FadeInDuration;
            source.IsFadeOutOnStop = unityAudioEvent.IsFadeOutOnStop;
            source.FadeOutDuration = unityAudioEvent.FadeOutDuration;

            source.IsRandomizeVolume = true;
            source.VolumeRange = unityAudioEvent.VolumeRange;

            source.IsRandomizePitch = true;
            source.PitchRange = unityAudioEvent.PitchRange;

            source.IsRandomizePlayTime = true;
            source.PlayTimeRange = unityAudioEvent.PlayTimeRange;

            source.IsRandomizePanning = true;
            source.PanningRange = unityAudioEvent.PanningRange;

            source.SpatialBlend = unityAudioEvent.SpatialBlend;
            source.DistanceRange = unityAudioEvent.DistanceRange;

            source.PlayCooldown = unityAudioEvent.PlayCooldown;
            source.PlayChance = 1f;
        }

        private ObjectPool<AudioSourceExtended> CreatePool(UnityAudioEventData data)
        {
            return new ObjectPool<AudioSourceExtended>(
                defaultCapacity: data.MaxInstanceCount,
                createFunc: () =>
                {
                    var source = Instantiate(
                        original: audioSourcePrefab,
                        parent: parentTransform
                    );

                    source.name = $"AudioSource ({data.Name})";
                    return source;
                },
                actionOnGet: source =>
                {
                    if (activeSourcesByEvent.TryGetValue(data, out var list) == false)
                    {
                        list = new List<AudioSourceExtended>();
                        activeSourcesByEvent[data] = list;
                    }

                    list.Add(source);

                    source.gameObject.SetActive(true);
                },
                actionOnRelease: source =>
                {
                    if (activeSourcesByEvent.TryGetValue(data, out var list))
                    {
                        list.Remove(source);
                    }

                    source.gameObject.SetActive(false);
                }
            );
        }
    }
}

#endif
