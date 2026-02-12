using System;
using System.Collections.Generic;
using UI.Presenters.Interfaces.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.Windows
{
    public class MainMenuWindow : WindowBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        
        private IMainMenuPresenter _presenter;
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                HandleChanged();
                
                _settingsButton.onClick.AddListener(_presenter.OpenSettings);
                _startButton.onClick.AddListener(_presenter.StartGame);
                _exitButton.onClick.AddListener(_presenter.ExitGame);
            }
        }

        public override void Hide(Action endCallback = null)
        {
            PresenterUtils.Teardown(ref _presenter, HandleChanged, startAction: () =>
            {
                _settingsButton.onClick.RemoveListener(_presenter.OpenSettings);
                _startButton.onClick.RemoveListener(_presenter.StartGame);
                _exitButton.onClick.RemoveListener(_presenter.ExitGame);
            });
            
            base.Hide(endCallback);
        }
        
        private void HandleChanged()
        {
            
        }
    }
}