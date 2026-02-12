using System;
using System.Collections.Generic;
using Services.GameCard;
using UI.Presenters.Interfaces.HUD;
using UI.View.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace UI.View.Windows
{
    public class HUD : WindowBase
    {
        [Header("Buttons")]
        [SerializeField] private Button _pauseButton;
        
        [Header("Cards")]
        [SerializeField] private Transform _cardsContainer;
        [SerializeField] private CardWidget _cardPrefab;
        
        private IHUDPresenter _presenter;
        private List<CardWidget> _spawnedCards = new();
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                HandleChanged();
                
                _presenter.Changed += HandleChanged; 
                
                _pauseButton.onClick.AddListener(_presenter.Pause);
                
                SpawnCards();
            }
        }

        public override void Hide(Action endCallback = null)
        {
            PresenterUtils.Teardown(ref _presenter, HandleChanged, startAction: () =>
            {
                _presenter.Changed -= HandleChanged; 
                
                _pauseButton.onClick.RemoveListener(_presenter.Pause);
                
                ClearCards();
            });
            
            base.Hide(endCallback);
        }
        
        private void HandleChanged()
        {
            SpawnCards();
        }

        private void SpawnCards()
        {
            ClearCards();
            
            List<CardPresenter> cards = _presenter.GetCards();
            foreach (CardPresenter cardData in cards)
            {
                CardWidget cardWidget = Instantiate(_cardPrefab, _cardsContainer);
                cardWidget.Show(cardData);
                _spawnedCards.Add(cardWidget);
            }
        }

        private void ClearCards()
        {
            foreach (CardWidget card in _spawnedCards)
            {
                Destroy(card.gameObject);
            }
            _spawnedCards.Clear();
        }
    }
}