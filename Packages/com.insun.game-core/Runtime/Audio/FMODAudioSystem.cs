#if FMOD_INSTALLED

using System;
using FMODUnity;
using UnityEngine;

namespace InSun.GameCore.Audio
{
    public sealed class FMODAudioSystem : MonoBehaviour, IAudioSystem
    {
        [Header("Banks")]
        [SerializeField]
        private StudioBankLoader bankLoader;

        [Header("Global Parameters")]
        [SerializeField]
        [ParamRef]
        private string masterVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string musicVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string sfxVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string ambienceVolumeParameter;

        [SerializeField]
        [ParamRef]
        private string voiceVolumeParameter;

        public bool IsLoading
        {
            get
            {
                if (RuntimeManager.HaveAllBanksLoaded == false)
                {
                    return true;
                }

                foreach (var bank in bankLoader.Banks)
                {
                    if (RuntimeManager.HasBankLoaded(bank) == false)
                    {
                        return true;
                    }
                }

                if (RuntimeManager.AnySampleDataLoading())
                {
                    return true;
                }

                return false;
            }
        }

        public void Load()
        {
            bankLoader.Load();
        }

        public void UnLoad()
        {
            bankLoader.Unload();
        }

        private void Start()
        {
            InitializeGlobalVolumeParameters();
        }

        public float GetVolume(VolumeType type)
        {
            return ReadVolume(type);
        }

        public void SetVolume(VolumeType type, float volume)
        {
            var clampedVolume = GetNormalizedVolume(volume);

            switch (type)
            {
                case VolumeType.Master:
                {
                    SetParameterValue(masterVolumeParameter, clampedVolume);
                    break;
                }
                case VolumeType.Music:
                {
                    SetParameterValue(musicVolumeParameter, clampedVolume);
                    break;
                }
                case VolumeType.SFX:
                {
                    SetParameterValue(sfxVolumeParameter, clampedVolume);
                    break;
                }
                case VolumeType.Ambience:
                {
                    SetParameterValue(ambienceVolumeParameter, clampedVolume);
                    break;
                }
                case VolumeType.Voice:
                {
                    SetParameterValue(voiceVolumeParameter, clampedVolume);
                    break;
                }
                default:
                {
                    Debug.LogWarning($"Unsupported volume type: {type}", this);
                    break;
                }
            }

            WriteVolume(type, clampedVolume);
        }

        public float GetParameter(GetAudioParameterArgs args)
        {
            return 0f;
        }

        public void SetParameter(SetAudioParameterArgs args)
        {
        }

        public IAudioInstance Play(PlayAudioEventArgs args)
        {
            return DefaultAudioInstance.Instance;
        }

        private void InitializeGlobalVolumeParameters()
        {
            SetParameterValue(masterVolumeParameter, ReadVolume(VolumeType.Master));
            SetParameterValue(musicVolumeParameter, ReadVolume(VolumeType.Music));
            SetParameterValue(sfxVolumeParameter, ReadVolume(VolumeType.SFX));
            SetParameterValue(ambienceVolumeParameter, ReadVolume(VolumeType.Ambience));
            SetParameterValue(voiceVolumeParameter, ReadVolume(VolumeType.Voice));
        }

        private void SetParameterValue(string parameterName, float value)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                return;
            }

            RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
        }

        private static float GetNormalizedVolume(float volume)
        {
            var clampedVolume = Mathf.Clamp(volume, 0f, 1f);
            var roundVolume = (float)Math.Round(clampedVolume, 2);

            return roundVolume;
        }

        private static float ReadVolume(VolumeType type, float defaultVolume = 1f)
        {
            return PlayerPrefs.GetFloat($"{nameof(FMODAudioSystem)}_{type.ToString()}", defaultVolume);
        }

        private static void WriteVolume(VolumeType type, float volume)
        {
            PlayerPrefs.SetFloat($"{nameof(FMODAudioSystem)}_{type.ToString()}", volume);
        }
    }
}

#endif
