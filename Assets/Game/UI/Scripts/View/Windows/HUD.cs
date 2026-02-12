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
        
        [Header("Drag Feedback")]
        [SerializeField] private GameObject _worldCursorPrefab;
        [SerializeField] private float _dragThresholdY = 200f;
        [SerializeField] private RectTransform _containerRect;
        
        private IHUDPresenter _presenter;
        private List<CardWidget> _spawnedCards = new();
        
        private Vector2 _shownPosition;
        private Vector2 _hiddenPosition;
        private bool _isInitialized;
        
        private GameObject _worldCursorInstance;
        private CardWidget _currentDraggingCard;
        
        public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
        {
            base.Show(extraData, endCallback);

            if (PresenterUtils.Setup(extraData,
                    key: Id,
                    subscribeAction: HandleChanged,
                    presenterField: ref _presenter))
            {
                SetupContainerInteraction();
                SetupWorldCursor();
                
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
                
                if (_worldCursorInstance != null)
                {
                    Destroy(_worldCursorInstance);
                    _worldCursorInstance = null;
                }
                
                // Restore position for next show
                if (_containerRect != null)
                {
                    _containerRect.DOKill();
                    _containerRect.anchoredPosition = _shownPosition;
                }
            });
            
            base.Hide(endCallback);
        }
        
        private void SetupWorldCursor()
        {
            if (_worldCursorPrefab != null && _worldCursorInstance == null)
            {
                _worldCursorInstance = Instantiate(_worldCursorPrefab);
                _worldCursorInstance.SetActive(false);
            }
        }
        
        private void SetupContainerInteraction()
        {
            if (!_isInitialized)
            {
                _shownPosition = _containerRect.anchoredPosition;
                _hiddenPosition = _shownPosition - new Vector2(0, _hiddenOffsetY);
                _isInitialized = true;
            }
            else
            {
                _containerRect.anchoredPosition = _shownPosition;
            }

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
            
            // If dragging, don't hide
            if (_currentDraggingCard != null) return;
            
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
                    UnsubscribeCard(cardWidget);
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
                
                SubscribeCard(cardWidget);
                
                _spawnedCards.Add(cardWidget);
            }
        }
        
        private void SubscribeCard(CardWidget card)
        {
            card.OnPutOnField += _presenter.PutOnField;
            card.OnPutOnCard += _presenter.PutOnCard;
            card.OnDragStart += () => OnCardDragStart(card);
            card.OnDragUpdate += OnCardDragUpdate;
            card.OnDragEnd += OnCardDragEnd;
        }
        
        private void UnsubscribeCard(CardWidget card)
        {
            card.OnPutOnField -= _presenter.PutOnField;
            card.OnPutOnCard -= _presenter.PutOnCard;
            // We can't easily unsubscribe anonymous delegates or method groups with parameters this way without storing them
            // But since the object is being destroyed, it's less critical, though good practice.
            // For simplicity in this context, we rely on the object destruction.
            // A cleaner way would be to have methods in HUD that take the card as arg, but CardWidget events don't pass 'this' except for PutOn...
            // Let's just leave it for now as the card is destroyed.
        }

        private void OnCardDragStart(CardWidget card)
        {
            _currentDraggingCard = card;
            SetContainerState(true); // Ensure container stays up
        }

        private void OnCardDragUpdate(Vector2 screenPos)
        {
            if (_currentDraggingCard == null) return;

            bool isAboveThreshold = screenPos.y > _dragThresholdY;
            
            _currentDraggingCard.SetGhostState(isAboveThreshold);
            
            if (isAboveThreshold)
            {
                if (_worldCursorInstance != null)
                {
                    _worldCursorInstance.SetActive(true);
                    Ray ray = Camera.main.ScreenPointToRay(screenPos);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        _worldCursorInstance.transform.position = hit.point;
                    }
                }
            }
            else
            {
                if (_worldCursorInstance != null)
                {
                    _worldCursorInstance.SetActive(false);
                }
            }
        }

        private void OnCardDragEnd()
        {
            _currentDraggingCard = null;
            if (_worldCursorInstance != null)
            {
                _worldCursorInstance.SetActive(false);
            }
            
            // Check if we should hide the container (if mouse is not over it)
            // Simple check: if mouse is low enough? Or just rely on PointerExit which might have fired or not.
            // Actually, if we dragged out, PointerExit might have fired but we forced it to stay open.
            // Let's check if the mouse is currently over the container rect.
            if (!RectTransformUtility.RectangleContainsScreenPoint(_containerRect, Input.mousePosition, null)) // null camera for Overlay
            {
                SetContainerState(false);
            }
        }

        private void ClearCards()
        {
            foreach (CardWidget card in _spawnedCards)
            {
                UnsubscribeCard(card); // Best effort
                Destroy(card.gameObject);
            }
            _spawnedCards.Clear();
        }
    }
}