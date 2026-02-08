using Services.Audio;
using Services.GameControl;
using Services.GameStates.StateMachine.Interfaces;
using UI.Presenters.Implementations.HUD;
using UI.View.Windows;

namespace Services.GameStates.StateMachine
{
    public class GameState : IState
    {
        private readonly GameControlService _gameController;
        private readonly UISystem _uiSystem;
        private readonly AudioService _audioService;
        
        public GameState(
            UISystem uiSystem,
            GameControlService gameController,
            AudioService audioService)
        {
            _uiSystem = uiSystem;
            _gameController = gameController;
            _audioService = audioService;
        }
        
        public void Enter()
        {
            _audioService.PlayBattleMusic();
            _gameController.StartGameSession();
            _uiSystem.Start<HUDPresenter, HUD>(GlobalKeys.UI.Window.HUD);
        }

        public void Exit()
        {
            _gameController.StopGameSession();
            _uiSystem.Stop(GlobalKeys.UI.Window.HUD);
        }
    }
}