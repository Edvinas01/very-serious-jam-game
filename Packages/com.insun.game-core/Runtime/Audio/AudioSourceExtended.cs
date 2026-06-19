#if UNITY_AUDIO_INSTALLED

using System;
using System.Collections.Generic;
using System.Linq;
using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace InSun.GameCore.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioSourceExtended : MonoBehaviour, IAudioInstance
    {
        [Header("Features")]
        [SerializeField]
        private bool isPlayOnStart;

        [SerializeField]
        private bool isPlayOnTop;

        [SerializeField]
        [Min(0f)]
        private float playCooldown;

        [SerializeField]
        [Range(0f, 1f)]
        private float playChance = 1f;

        [ShowIf(nameof(IsShowDestroyOnFinishedEditor))]
        [SerializeField]
        private bool isDestroyOnFailRandom;

        [SerializeField]
        private bool isDestroyOnFinished;

        [Header("Fade")]
        [SerializeField]
        private bool isFadeInOnPlay;

        [ShowIf(nameof(isFadeInOnPlay))]
        [Min(0f)]
        [SerializeField]
        private float fadeInDuration = 0.5f;

        [SerializeField]
        private bool isFadeOutOnStop;

        [ShowIf(nameof(isFadeOutOnStop))]
        [Min(0f)]
        [SerializeField]
        private float fadeOutDuration = 0.5f;

        [Header("Randomization: Clips")]
        [SerializeField]
        private List<AudioClip> randomClips;

        [Header("Randomization: Volume")]
        [SerializeField]
        private bool isRandomizeVolume;

        [ShowIf(nameof(isRandomizeVolume))]
        [SerializeField]
        private Vector2 volumeRange = new(1.0f, 1.2f);

        [Header("Randomization: Panning")]
        [SerializeField]
        private bool isRandomizePanning;

        [ShowIf(nameof(isRandomizePanning))]
        [SerializeField]
        private Vector2 panningRange = new(-1f, +1f);

        [Header("Randomization: Pitch")]
        [SerializeField]
        private bool isRandomizePitch;

        [ShowIf(nameof(isRandomizePitch))]
        [SerializeField]
        private Vector2 pitchRange = new(1.0f, 1.2f);

        [Header("Randomization: Play Time")]
        [SerializeField]
        private bool isRandomizePlayTime;

        [ShowIf(nameof(isRandomizePlayTime))]
        [SerializeField]
        private Vector2 playTimeRange = new(0f, 0.3f);

        [Header("Events")]
        [SerializeField]
        private UnityEvent onFinished;

        private IAudioSystem audioSystem;
        private AudioSource audioSource;
        private bool isPlaying;
        private float initialPitch;
        private float pitch;
        private float pitchMultiplier = 1f;
        private float cooldown;

        // Fade state
        private bool isFadingIn;
        private bool isFadingOut;
        private float fadeTimer;
        private float targetVolume;

        public bool IsPlaying => isPlaying;

        public float Duration
        {
            get
            {
                var clip = audioSource.clip;
                if (isPlaying && clip)
                {
                    return clip.length;
                }

                return 0f;
            }
        }

        public IReadOnlyList<AudioClip> RandomClips
        {
            get => randomClips;
            set => randomClips = value as List<AudioClip> ?? value.ToList();
        }

        public bool IsPlayOnTop
        {
            get => isPlayOnTop;
            set => isPlayOnTop = value;
        }

        public bool IsFadeInOnPlay
        {
            get => isFadeInOnPlay;
            set => isFadeInOnPlay = value;
        }

        public float FadeInDuration
        {
            get => fadeInDuration;
            set => fadeInDuration = value;
        }

        public bool IsFadeOutOnStop
        {
            get => isFadeOutOnStop;
            set => isFadeOutOnStop = value;
        }

        public float FadeOutDuration
        {
            get => fadeOutDuration;
            set => fadeOutDuration = value;
        }

        public bool IsRandomizeVolume
        {
            get => isRandomizeVolume;
            set => isRandomizeVolume = value;
        }

        public Vector2 VolumeRange
        {
            get => volumeRange;
            set
            {
                volumeRange = value;

                if (isRandomizeVolume)
                {
                    audioSource.volume = value.GetRandomFloat();
                }
            }
        }

        public bool IsRandomizePanning
        {
            get => isRandomizePanning;
            set => isRandomizePanning = value;
        }

        public Vector2 PanningRange
        {
            get => panningRange;
            set
            {
                panningRange = value;

                if (isRandomizePanning)
                {
                    audioSource.panStereo = value.GetRandomFloat();
                }
            }
        }

        public bool IsRandomizePitch
        {
            get => isRandomizePitch;
            set => isRandomizePitch = value;
        }

        public Vector2 PitchRange
        {
            get => pitchRange;
            set
            {
                pitchRange = value;

                if (isRandomizePitch)
                {
                    audioSource.pitch = value.GetRandomFloat();
                }
            }
        }

        public bool IsRandomizePlayTime
        {
            get => isRandomizePlayTime;
            set => isRandomizePlayTime = value;
        }

        public Vector2 PlayTimeRange
        {
            get => playTimeRange;
            set => playTimeRange = value;
        }

        public float PlayCooldown
        {
            get => playCooldown;
            set => playCooldown = value;
        }

        public float PlayChance
        {
            get => playChance;
            set => playChance = value;
        }

        public string Name
        {
            get
            {
                var clip = audioSource.clip;
                if (isPlaying && clip)
                {
                    return clip.name;
                }

                return name;
            }
        }

        private ObjectPool<AudioSourceExtended> pool;

        public ObjectPool<AudioSourceExtended> Pool
        {
            get => pool;
            set
            {
                if (value != null && isDestroyOnFinished)
                {
                    Debug.LogWarning(
                        $"Audio source '{name}' has {nameof(isDestroyOnFinished)} enabled but is assigned a pool so this setting is ignored",
                        this
                    );
                }

                pool = value;
            }
        }

        public AnimationCurve VolumeCurve
        {
            set => audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, value);
        }

        public float SpatialBlend
        {
            set => audioSource.spatialBlend = value;
        }

        public AudioRolloffMode AudioRolloffMode
        {
            set => audioSource.rolloffMode = value;
        }

        public Vector2 DistanceRange
        {
            set
            {
                audioSource.minDistance = value.x;
                audioSource.maxDistance = value.y;
            }
        }

        public AudioMixerGroup MixerGroup
        {
            set => audioSource.outputAudioMixerGroup = value;
        }

        public Transform FollowTransform { get; set; }

        public bool IsLoop
        {
            set => audioSource.loop = value;
        }

        public event Action OnFinished;

        public event Action<IAudioInstance> OnStopped;

        private void Awake()
        {
            audioSystem = Game.GetObject<IAudioSystem>();
            audioSource = GetComponent<AudioSource>();
            initialPitch = audioSource.pitch;
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
            StopImmediate();
        }

        public void SetCurve(AudioSourceCurveType type, AnimationCurve curve)
        {
            audioSource.SetCustomCurve(type, curve);
        }

        public void SetPitchMultiplier(float value)
        {
            pitchMultiplier = value;
            audioSource.pitch = pitch * pitchMultiplier;
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (cooldown > 0f && isPlayOnTop == false)
            {
                return;
            }

            if (isPlaying && isPlayOnTop == false)
            {
                return;
            }

            if (Random.value < 1f - playChance)
            {
                if (isDestroyOnFailRandom && pool == null)
                {
                    Destroy(this);
                    Destroy(audioSource);
                }

                return;
            }

            if (isPlaying)
            {
                if (audioSystem is UnityAudioSystem unityAudioSystem)
                {
                    unityAudioSystem.RemoveActiveAudioSource(this);
                }

                audioSource.Stop();
                OnStopped?.Invoke(this);
            }

            if (randomClips.TryGetRandom(out var randomClip))
            {
                audioSource.clip = randomClip;
            }

            // Determine target volume
            if (isRandomizeVolume)
            {
                targetVolume = Random.Range(volumeRange.x, volumeRange.y);
            }
            else
            {
                targetVolume = audioSource.volume;
            }

            // Set up fade in if enabled
            if (isFadeInOnPlay && fadeInDuration > 0f)
            {
                audioSource.volume = 0f;
                isFadingIn = true;
                fadeTimer = 0f;
            }
            else
            {
                audioSource.volume = targetVolume;
                isFadingIn = false;
            }

            isFadingOut = false;

            if (isRandomizePitch)
            {
                pitch = Random.Range(pitchRange.x, pitchRange.y) * pitchMultiplier;
            }
            else
            {
                pitch = initialPitch * pitchMultiplier;
            }

            if (isRandomizePlayTime)
            {
                var clip = audioSource.clip;
                var time = Mathf.Min(playTimeRange.y, clip.length);
                audioSource.time = Random.Range(playTimeRange.x, time);
            }

            if (isRandomizePanning)
            {
                audioSource.panStereo = panningRange.GetRandomFloat();
            }

            audioSource.pitch = pitch;
            audioSource.Play();

            {
                if (audioSystem is UnityAudioSystem unityAudioSystem)
                {
                    unityAudioSystem.AddActiveAudioSource(this);
                }
            }

            isPlaying = true;
            cooldown = playCooldown;
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            if (isPlaying == false)
            {
                return;
            }

            if (isFadeOutOnStop && fadeOutDuration > 0f)
            {
                isFadingOut = true;
                isFadingIn = false;
                fadeTimer = 0f;
            }
            else
            {
                StopImmediate();
            }
        }

        private void StopImmediate()
        {
            audioSource.Stop();
            OnStopped?.Invoke(this);

            if (audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.RemoveActiveAudioSource(this);
            }

            var isPlayingPrev = isPlaying;
            isPlaying = false;
            isFadingIn = false;
            isFadingOut = false;
            cooldown = 0f;

            if (isPlayingPrev && isDestroyOnFinished && pool == null)
            {
                Destroy(this);
                Destroy(audioSource);
                return;
            }

            var poolPrev = Pool;
            Pool = null;
            poolPrev?.Release(this);
        }

        public void Pause()
        {
            if (isPlaying && audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.RemoveActiveAudioSource(this);
            }

            audioSource.Pause();
        }

        public void UnPause()
        {
            if (isPlaying && audioSystem is UnityAudioSystem unityAudioSystem)
            {
                unityAudioSystem.AddActiveAudioSource(this);
            }

            audioSource.UnPause();
        }

        public void Tick(float dt)
        {
            if (FollowTransform)
            {
                transform.position = FollowTransform.position;
            }

            if (cooldown > 0f)
            {
                cooldown -= dt;
            }

            if (isFadingIn)
            {
                fadeTimer += dt;
                var normTime = Mathf.Clamp01(fadeTimer / fadeInDuration);
                audioSource.volume = Mathf.Lerp(0f, targetVolume, normTime);

                if (normTime >= 1f)
                {
                    isFadingIn = false;
                    audioSource.volume = targetVolume;
                }
            }

            if (isFadingOut)
            {
                fadeTimer += dt;
                var normTime = Mathf.Clamp01(fadeTimer / fadeOutDuration);
                audioSource.volume = Mathf.Lerp(targetVolume, 0f, normTime);

                if (normTime >= 1f)
                {
                    StopImmediate();
                    return;
                }
            }

            if (isFadingOut == false)
            {
                var isPlayingPrev = isPlaying;
                var isPlayingNext = audioSource.isPlaying;
                if (isPlayingPrev && isPlayingNext == false)
                {
                    if (audioSystem is UnityAudioSystem unityAudioSystem)
                    {
                        unityAudioSystem.RemoveActiveAudioSource(this);
                    }

                    OnFinished?.Invoke();
                    onFinished.Invoke();
                    isPlaying = false;

                    OnStopped?.Invoke(this);

                    if (isDestroyOnFinished && pool == null)
                    {
                        Destroy(this);
                        Destroy(audioSource);
                        return;
                    }

                    var poolPrev = Pool;
                    Pool = null;
                    poolPrev?.Release(this);
                }
                else
                {
                    isPlaying = isPlayingNext;
                }
            }
        }

        private bool IsShowDestroyOnFinishedEditor()
        {
            return playChance < 1f;
        }
    }
}
#endif
