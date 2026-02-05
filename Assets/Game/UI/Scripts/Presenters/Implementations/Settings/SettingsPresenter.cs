using System;
using Services.Audio;
using Services.SaveLoad.Data;
using Services.SaveLoad.Services;
using UI.Presenters.Interfaces.Settings;
using UnityEngine;
using Zenject;

namespace UI.Presenters.Implementations.Settings
{
    [PresenterImpl(typeof(ISettingsPresenter))]
    public class SettingsPresenter : ISettingsPresenter
    {
        public event Action Changed;
        
        public bool IsMusicMute => _audioService.IsMusicMuted();
        public bool IsSoundMute => _audioService.IsSoundMuted();
        
        public float MusicVolume => _audioService.GetRawMusicVolume();
        public float SoundVolume => _audioService.GetRawSoundVolume();

        private readonly AudioService _audioService;
        private readonly SaveLoadService _saveLoadService;
        private readonly UISystem _uiSystem;
        private readonly IInstantiator _instantiator;
        
        public SettingsPresenter(
            AudioService audioService,
            SaveLoadService saveLoadService,
            UISystem uiSystem,
            IInstantiator instantiator)
        {
            _audioService = audioService;
            _instantiator = instantiator;
            _uiSystem = uiSystem;
            _saveLoadService = saveLoadService;
        }
        
        public void Start()
        {
            _audioService.PlayOpenPopupSound();
        }

        public void Stop()
        {
            _audioService.SetPopupFilter(false);
        }

        public void Close()
        {
            SettingsData quickSaveData = _saveLoadService.LoadSettings(); 
            _saveLoadService.SaveSettings(quickSaveData);
            _uiSystem.Stop(GlobalKeys.UI.Popup.SettingsPopup);
        }

        public void SetSoundVolume(float volume)
        {
            _audioService.SetSoundVolume(volume);
            _audioService.PlayButtonSound();
            Changed?.Invoke();
        }

        public void SetMusicVolume(float volume)
        {
            _audioService.SetMusicVolume(volume);
            Changed?.Invoke();
        }

        public void SetSoundMute(bool unmute)
        {
            _audioService.SetSoundMute(!unmute);
            _audioService.PlayButtonSound();
            Changed?.Invoke();
        }

        public void SetMusicMute(bool unmute)
        {
            _audioService.SetMusicMute(!unmute);
            Changed?.Invoke();
        }

        public void Restore()
        {
            //_uiSystem.Start<RestoreConfirmPresenter, ConfirmPopup>(GlobalKeys.UI.Popup.ConfirmPopup);
            Changed?.Invoke();
        }
    }
}