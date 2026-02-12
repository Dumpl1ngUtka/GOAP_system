using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Services.GameCard;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _cardsCountText;
        
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
            UpdateCardsCount();
        }

        private void UpdateCardsCount()
        {
            if (_cardsCountText != null)
            {
                _cardsCountText.text = $"{_presenter.GetCards().Count}/{_presenter.GetMaxCardsCount()}";
            }
        }

        private void SpawnCards()
        {
            List<CardPresenter> cards = _presenter.GetCards();
            
            // Remove cards that are no longer in the list
            for (int i = _spawnedCards.Count - 1; i >= 0; i--)
            {
                CardWidget cardWidget = _spawnedCards[i];
                if (!cards.Contains(cardWidget.Presenter))
                {
                    _spawnedCards.RemoveAt(i);
                    Destroy(cardWidget.gameObject);
                }
            }

            // Add new cards
            foreach (CardPresenter cardData in cards)
            {
                if (_spawnedCards.Any(c => c.Presenter == cardData))
                {
                    continue;
                }
                
                CardWidget cardWidget = Instantiate(_cardPrefab, _cardsContainer);
                cardWidget.Show(cardData);
                cardWidget.transform.localScale = Vector3.zero;
                cardWidget.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
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