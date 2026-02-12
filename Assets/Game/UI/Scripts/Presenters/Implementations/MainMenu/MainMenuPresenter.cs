using System;
using Services.GameStates;
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
        private readonly GameStateService _gameStateService;

        public MainMenuPresenter(
            UISystem uiSystem,
            GameStateService gameStateService)
        {
            _uiSystem = uiSystem;
            _gameStateService = gameStateService;
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
            _gameStateService.SetGameState();
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}