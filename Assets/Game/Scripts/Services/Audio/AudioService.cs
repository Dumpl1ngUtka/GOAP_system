using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Services.SaveLoad.Data;
using Services.SaveLoad.Services;
using UnityEngine;
#if YandexGamesPlatform_yg
using YG;
#endif

namespace Services.Audio
{
    public class AudioService
    {
        public event Action Changed;

        private const float NormalFreq = 22000f;
        private const float MuffledFreq = 800f;
        private const float TransitionDuration = 0.25f;
        private const float MusicVolumeFilter = 0.2f;
        
        private readonly AudioSource _musicSource;
        private readonly AudioLowPassFilter _lowPassFilter;
        private readonly AudioSource _soundSource;
        private readonly SaveLoadService _saveLoadService;
        private readonly AudioConfig _audioConfig;
        
        private float _musicVolumeFilterModifier = 1;

        private float _cachedSoundVolume;
        private float _cachedMusicVolume;
        private bool _cachedSoundMute;
        private bool _cachedMusicMute;

        public float MusicVolume => _cachedMusicMute ? 0 : _cachedMusicVolume * _musicVolumeFilterModifier;
        public float SoundVolume => _cachedSoundMute ? 0 : _cachedSoundVolume;

        public float GetRawMusicVolume() => _cachedMusicVolume;
        public float GetRawSoundVolume() => _cachedSoundVolume;
        public bool IsMusicMuted() => _cachedMusicMute;
        public bool IsSoundMuted() => _cachedSoundMute;

        public AudioService(
            SaveLoadService saveLoadService,
            GameConfig gameConfig)
        {
            GameObject audioRoot = new("[AudioService_Root]");
            UnityEngine.Object.DontDestroyOnLoad(audioRoot);
            
            _soundSource = InitSoundSource(audioRoot);
            _musicSource = InitMusicSource(audioRoot);
            _lowPassFilter = InitLowPassFilter(audioRoot);
            
            _saveLoadService = saveLoadService;
            _audioConfig = gameConfig.AudioConfig;
        }

        public async UniTask LoadSettings()
        {
            SettingsData data = _saveLoadService.LoadSettings();
            _cachedSoundVolume = data.SoundVolume;
            _cachedMusicVolume = data.MusicVolume;
            _cachedSoundMute = data.SoundMute;
            _cachedMusicMute = data.MusicMute;
            
            UpdatePlayingSourcesVolume();
        }

        public void SetSoundVolume(float volume)
        {
            _cachedSoundVolume = volume;
            UpdatePlayingSourcesVolume();
            
            SettingsData data = _saveLoadService.LoadSettings();
            data.SoundVolume = volume;
            _saveLoadService.QuickSaveSettings(data);
            
            Changed?.Invoke();
        }

        public void SetMusicVolume(float volume)
        {
            _cachedMusicVolume = volume;
            UpdatePlayingSourcesVolume();
            
            SettingsData data = _saveLoadService.LoadSettings();
            data.MusicVolume = volume;
            _saveLoadService.QuickSaveSettings(data);
            
            Changed?.Invoke();
        }

        public void SetSoundMute(bool mute)
        {
            _cachedSoundMute = mute;
            UpdatePlayingSourcesVolume();
            
            SettingsData data = _saveLoadService.LoadSettings();
            data.SoundMute = mute;
            _saveLoadService.QuickSaveSettings(data);
            
            Changed?.Invoke();
        }

        public void SetMusicMute(bool mute)
        {
            _cachedMusicMute = mute;
            UpdatePlayingSourcesVolume();
            
            SettingsData data = _saveLoadService.LoadSettings();
            data.MusicMute = mute;
            _saveLoadService.QuickSaveSettings(data);
            
            Changed?.Invoke();
        }

        public void UpdatePlayingSourcesVolume()
        {
            _musicSource.volume = MusicVolume;
            _soundSource.volume = SoundVolume;
        }
        
        public void PlayButtonSound() => Play(_soundSource, _audioConfig.DefaultButtonSound, SoundVolume);
        public void PlayOpenPopupSound() => Play(_soundSource, _audioConfig.OpenPopupSound, SoundVolume);

        public void SetPopupFilter(bool isEnabled)
        {
            float targetFreq = isEnabled ? MuffledFreq : NormalFreq;
            float targetVolume = isEnabled ? MusicVolumeFilter : 1f;
            
            DOTween.To(() => _lowPassFilter.cutoffFrequency, 
                    x => _lowPassFilter.cutoffFrequency = x, 
                    targetFreq, TransitionDuration)
                .SetUpdate(true);
            
            DOTween.To(() => _musicVolumeFilterModifier, 
                    x => _musicVolumeFilterModifier = x, 
                    targetVolume, TransitionDuration)
                .SetUpdate(true);
        }
        
        public void PlayMainMenuMusic() => Play(_musicSource, _audioConfig.BattleBackgroundMusic, MusicVolume);
        public void PlayBattleMusic() => Play(_musicSource, _audioConfig.BattleBackgroundMusic, MusicVolume);
        public void StopBackgroundMusic() => _musicSource.Stop();

        private void Play(AudioSource source, AudioClip clip, float volume)
        {
            if (source.clip == clip && source.isPlaying) 
                return;

            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
        
        private AudioSource InitSoundSource(GameObject audioRoot)
        {
            AudioSource source = audioRoot.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.bypassEffects = true;
            return source;
        }
        
        private AudioSource InitMusicSource(GameObject audioRoot)
        {
            AudioSource musicSource = audioRoot.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            return musicSource;
        }

        private AudioLowPassFilter InitLowPassFilter(GameObject audioRoot)
        {
            AudioLowPassFilter filter = audioRoot.AddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = 22000;
            return filter;
        }
        
    }
}