using Services.Audio;
using Services.GameStates.StateMachine.Interfaces;
using UI.Presenters.Implementations.MainMenu;
using UI.View.Windows;

namespace Services.GameStates.StateMachine
{
    public class MainMenuState : IState
    {
        private readonly UISystem _uiSystem;
        private readonly AudioService _audioService;
        private readonly SceneLoader.SceneLoader _sceneLoader;
        
        public MainMenuState(
            UISystem uiSystem, 
            AudioService audioService,
            SceneLoader.SceneLoader sceneLoader)
        {
            _uiSystem = uiSystem;
            _audioService = audioService;
            _sceneLoader = sceneLoader;
        }
        
        public void Enter()
        {
            _sceneLoader.Load(GlobalKeys.Scene.MainMenuScene, OnLoaded);
        }

        private void OnLoaded()
        {
            _audioService.PlayMainMenuMusic();
            _uiSystem.Start<MainMenuPresenter, MainMenuWindow>(GlobalKeys.UI.Window.MainMenuWindow);
        }

        public void Exit()
        {
            _uiSystem.Stop(GlobalKeys.UI.Window.MainMenuWindow);
        }
    }
}