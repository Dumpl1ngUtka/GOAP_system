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
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _statisticsButton;
        
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
                _shopButton.onClick.AddListener(_presenter.OpenShop);
                _statisticsButton.onClick.AddListener(_presenter.OpenStatistics);
            }
        }

        public override void Hide(Action endCallback = null)
        {
            PresenterUtils.Teardown(ref _presenter, HandleChanged, startAction: () =>
            {
                _settingsButton.onClick.RemoveListener(_presenter.OpenSettings);
                _startButton.onClick.RemoveListener(_presenter.StartGame);
                _shopButton.onClick.RemoveListener(_presenter.OpenShop);
                _statisticsButton.onClick.RemoveListener(_presenter.OpenStatistics);
            });
            
            base.Hide(endCallback);
        }
        
        private void HandleChanged()
        {
            
        }
    }
}