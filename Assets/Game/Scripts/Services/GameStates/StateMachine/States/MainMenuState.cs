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
        
        public MainMenuState(
            UISystem uiSystem, 
            AudioService audioService)
        {
            _uiSystem = uiSystem;
            _audioService = audioService;
        }
        
        public void Enter()
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