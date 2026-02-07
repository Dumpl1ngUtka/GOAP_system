using System;
using Services.GameCard;
using UI.Presenters.Interfaces.HUD;

namespace UI.Presenters.Implementations.HUD
{
    [PresenterImpl(typeof(IHUDPresenter))]
    public class HUDPresenter : IHUDPresenter
    {
        public event Action Changed;
        
        private readonly GameCardService _gameCardService;
        private readonly UISystem _uiSystem;

        public HUDPresenter(
            UISystem uiSystem,
            GameCardService gameCardService)
        {
            _uiSystem = uiSystem;
            _gameCardService = gameCardService;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Pause()
        {
            //_uiSystem.Start<PausePopup, PausePresenter>(GlobalKeys.UI.Popup.PausePopup);
        }
    }
}