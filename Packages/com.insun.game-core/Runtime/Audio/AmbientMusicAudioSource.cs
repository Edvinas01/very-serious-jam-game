#if UNITY_AUDIO_INSTALLED
using System.Collections;
using System.Collections.Generic;
using InSun.GameCore.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InSun.GameCore.Audio
{
    internal sealed class AmbientMusicAudioSource : MonoBehaviour
    {
        [SerializeField]
        private Vector2 pauseBetweenTracks = new(10f, 60f);

        [SerializeField]
        private List<AudioSourceExtended> tracks;

        private readonly List<AudioSourceExtended> shuffledTracks = new();
        private int currentTrackIndex;

        private void Awake()
        {
            foreach (var track in tracks)
            {
                shuffledTracks.Add(track);
            }
        }

        private void Start()
        {
            if (tracks == null || tracks.Count == 0)
            {
                Debug.LogWarning("No tracks assigned!", this);
                enabled = false;
                return;
            }

            ShuffleTracks();

            var pauseDuration = RandomUtilities.GetRandomFloat(pauseBetweenTracks);
            pauseDuration = Mathf.Max(0, pauseDuration);

            StartCoroutine(WaitAndPlayNextRoutine(pauseDuration));
        }

        private void ShuffleTracks()
        {
            for (var index = shuffledTracks.Count - 1; index > 0; index--)
            {
                var randomIndex = Random.Range(0, index + 1);
                (shuffledTracks[index], shuffledTracks[randomIndex]) = (shuffledTracks[randomIndex], shuffledTracks[index]);
            }

            currentTrackIndex = 0;
        }

        private void PlayNextTrack()
        {
            if (shuffledTracks == null || shuffledTracks.Count == 0)
            {
                return;
            }

            var track = shuffledTracks[currentTrackIndex];

            track.OnFinished += OnTrackFinished;

            track.Play();

            Debug.Log($"Playing {track.Name} ({currentTrackIndex + 1}/{shuffledTracks.Count})", this);
        }

        private void OnTrackFinished()
        {
            shuffledTracks[currentTrackIndex].OnFinished -= OnTrackFinished;

            currentTrackIndex++;

            if (currentTrackIndex >= shuffledTracks.Count)
            {
                ShuffleTracks();
            }

            var pauseDuration = RandomUtilities.GetRandomFloat(pauseBetweenTracks);
            pauseDuration = Mathf.Max(0, pauseDuration);

            StartCoroutine(WaitAndPlayNextRoutine(pauseDuration));
        }

        private IEnumerator WaitAndPlayNextRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayNextTrack();
        }

        private void OnDestroy()
        {
            if (shuffledTracks != null && currentTrackIndex < shuffledTracks.Count)
            {
                shuffledTracks[currentTrackIndex].OnFinished -= OnTrackFinished;
            }
        }
    }
}

#endif
