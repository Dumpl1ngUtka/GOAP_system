using System;
using Config;
using Services.GameStates.StateMachine;

namespace Services.GameStates
{
    public class GameStateService
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameStateService(
            GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void SetBootstrapState()
        {
            _gameStateMachine.Enter<BootstrapState>();
        }

        public void SetMainMenuState()
        {
            _gameStateMachine.Enter<MainMenuState>();
        }

        public void SetGameState()
        {
            _gameStateMachine.Enter<GameState>();
        }
    }
}