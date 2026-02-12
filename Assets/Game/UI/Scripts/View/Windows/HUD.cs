using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Services.GameCard;
using TMPro;
using UI.Presenters.Interfaces.HUD;
using UI.View.Widgets;
using UnityEngine;
using UnityEngine.EventSystems;
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
        
        [Header("Animation")]
        [SerializeField] private float _hiddenOffsetY = 150f;
        [SerializeField] private float _moveDuration = 0.3f;
        
        private IHUDPresenter _presenter;
        private List<CardWidget> _spawnedCards = new();
        
        private RectTransform _containerRect;
        private Vector2 _shownPosition;
        private Vector2 _hiddenPosition;
        private bool _isInitialized;
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                SetupContainerInteraction();
                
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
                
                // Restore position for next show
                if (_containerRect != null)
                {
                    _containerRect.DOKill();
                    _containerRect.anchoredPosition = _shownPosition;
                }
            });
            
            base.Hide(endCallback);
        }
        
        private void SetupContainerInteraction()
        {
            _containerRect = _cardsContainer.GetComponent<RectTransform>();
            
            // Capture initial position as the "Shown" position
            // We assume the prefab is set up in the "Shown" state
            if (!_isInitialized)
            {
                _shownPosition = _containerRect.anchoredPosition;
                _hiddenPosition = _shownPosition - new Vector2(0, _hiddenOffsetY);
                _isInitialized = true;
            }
            else
            {
                // Ensure we start at the correct position before animating
                _containerRect.anchoredPosition = _shownPosition;
            }

            // Ensure we have a raycast target for events
            Image img = _cardsContainer.GetComponent<Image>();
            if (img == null)
            {
                img = _cardsContainer.gameObject.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0); // Transparent
            }
            img.raycastTarget = true;

            // Setup Events
            EventTrigger trigger = _cardsContainer.GetComponent<EventTrigger>();
            if (trigger == null) trigger = _cardsContainer.gameObject.AddComponent<EventTrigger>();
            
            trigger.triggers.Clear();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entryEnter.callback.AddListener((data) => SetContainerState(true));
            trigger.triggers.Add(entryEnter);

            EventTrigger.Entry entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entryExit.callback.AddListener((data) => SetContainerState(false));
            trigger.triggers.Add(entryExit);
            
            // Start hidden
            SetContainerState(false);
        }

        private void SetContainerState(bool show)
        {
            if (_containerRect == null) return;
            
            _containerRect.DOKill();
            _containerRect.DOAnchorPos(show ? _shownPosition : _hiddenPosition, _moveDuration)
                .SetEase(Ease.OutQuad);
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
                _cardsCountText.text = _presenter.CardsCountText;
                _cardsCountText.color = _presenter.CardsCountColor;
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