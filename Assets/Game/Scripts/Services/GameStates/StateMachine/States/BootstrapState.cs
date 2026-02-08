using Services.GameStates.StateMachine.Interfaces;

namespace Services.GameStates.StateMachine
{
    public class BootstrapState : IState
    {
        private readonly GameStateService _gameStateService;
        private readonly SceneLoader.SceneLoader _sceneLoader;
        private UISystem _uiSystem;
        
        public BootstrapState(
            GameStateService gameStateService,
            UISystem uiSystem,
            SceneLoader.SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _gameStateService = gameStateService;
            _uiSystem = uiSystem;
        }
        
        public void Enter()
        {
            LoadGame();
        }
        
        public void Exit()
        {
        }
        
        private void LoadGame()
        {
            _sceneLoader.Load(GlobalKeys.Scene.MainMenuScene, _gameStateService.SetMainMenuState);
        }
    }
}