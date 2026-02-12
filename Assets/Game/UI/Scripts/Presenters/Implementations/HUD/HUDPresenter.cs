using System;
using System.Collections.Generic;
using Services.GameCard;
using Services.GameControl;
using UI.Presenters.Interfaces.HUD;
using UnityEngine;

namespace UI.Presenters.Implementations.HUD
{
    [PresenterImpl(typeof(IHUDPresenter))]
    public class HUDPresenter : IHUDPresenter
    {
        public event Action Changed;

        private List<CardPresenter> _cards;
        
        private readonly GameCardService _gameCardService;
        private readonly GameControlService _gameControlService;
        private readonly UISystem _uiSystem;

        public HUDPresenter(
            UISystem uiSystem,
            GameControlService gameControlService,
            GameCardService gameCardService)
        {
            _uiSystem = uiSystem;
            _gameControlService = gameControlService;
            _gameCardService = gameCardService;
        }

        public void Start()
        {
            _gameControlService.SpawnCard += OnCardSpawn;

            _cards = new List<CardPresenter>();
        }
        
        public void Stop()
        {
            _gameControlService.SpawnCard -= OnCardSpawn;
        }
        
        public void Pause()
        {
            //_uiSystem.Start<PausePopup, PausePresenter>(GlobalKeys.UI.Popup.PausePopup);
        }

        public List<CardPresenter> GetCards() => _cards;
        
        private void OnCardSpawn()
        {
            CardPresenter card = _gameCardService.GetRandomCardByType(_gameControlService.CurrentCardType);
            _cards.Add(card);
            Changed?.Invoke();
            Debug.Log("Spawn");
        }
    }
}