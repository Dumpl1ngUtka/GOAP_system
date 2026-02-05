using System;
using System.Collections.Generic;
using UI.Presenters.Interfaces.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Scripts.View.Popups
{
    public class SettingsPopup : PopupBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _closeButton;
        [Header("Toggles")]
        [SerializeField] private Toggle _muteMusicButton;
        [SerializeField] private Toggle _muteSoundButton;
        [Header("Sliders")]
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _soundVolumeSlider;
        
        private ISettingsPresenter _presenter;
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                HandleChanged();
                
                _presenter.Changed += HandleChanged; 
                
                _closeButton.onClick.AddListener(_presenter.Close);
                _muteSoundButton.onValueChanged.AddListener(_presenter.SetSoundMute);
                _muteMusicButton.onValueChanged.AddListener(_presenter.SetMusicMute);
                
                _musicVolumeSlider.onValueChanged.AddListener(_presenter.SetMusicVolume);
                _soundVolumeSlider.onValueChanged.AddListener(_presenter.SetSoundVolume);
            }
        }

        public override void Hide(Action endCallback = null)
        {
            PresenterUtils.Teardown(ref _presenter, HandleChanged, startAction: () =>
            {
                _presenter.Changed -= HandleChanged; 
                
                _closeButton.onClick.RemoveListener(_presenter.Close);
                _muteSoundButton.onValueChanged.RemoveListener(_presenter.SetSoundMute);
                _muteMusicButton.onValueChanged.RemoveListener(_presenter.SetMusicMute);
                
                _musicVolumeSlider.onValueChanged.RemoveListener(_presenter.SetMusicVolume);
                _soundVolumeSlider.onValueChanged.RemoveListener(_presenter.SetSoundVolume);
            });
            
            base.Hide(endCallback);
        }
        
        private void HandleChanged()
        {
            _muteMusicButton.isOn = !_presenter.IsMusicMute;
            _muteSoundButton.isOn = !_presenter.IsSoundMute;

            _soundVolumeSlider.value = _presenter.SoundVolume;
            _musicVolumeSlider.value = _presenter.MusicVolume;
        }
    }
}