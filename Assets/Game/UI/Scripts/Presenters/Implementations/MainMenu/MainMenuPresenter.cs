using System;
using UI.Presenters.Implementations.Settings;
using UI.Presenters.Interfaces.MainMenu;
using UI.Scripts.View.Popups;

namespace UI.Presenters.Implementations.MainMenu
{
    [PresenterImpl(typeof(IMainMenuPresenter))]
    public class MainMenuPresenter : IMainMenuPresenter
    {
        public event Action Changed;
        
        private readonly UISystem _uiSystem;

        public MainMenuPresenter(
            UISystem uiSystem)
        {
            _uiSystem = uiSystem;
        }
        
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void OpenSettings()
        {
            _uiSystem.Start<SettingsPresenter, SettingsPopup>(GlobalKeys.UI.Popup.SettingsPopup);
        }

        public void StartGame()
        {
            //_uiSystem.Start<SelectModePresenter, SelectModePopup>(GlobalKeys.UI.Popup.SelectModePopup);
        }

        public void OpenShop()
        {
            //_uiSystem.Start<CustomizePresenter, CustomizePopup>(GlobalKeys.UI.Popup.CustomizePopup);
        }

        public void OpenStatistics()
        {
            //_uiSystem.Start<RunHistoryPresenter, RunHistoryPopup>(GlobalKeys.UI.Popup.RunHistoryPopup);
        }
    }
}