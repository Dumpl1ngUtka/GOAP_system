using System;
using System.Collections.Generic;
using DG.Tweening;
using Services.GameCard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UI.View.Widgets
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CardWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private Button _button;
        
        [Header("Animation")]
        [SerializeField] private float _hoverScale = 1.2f;
        [SerializeField] private float _scaleDuration = 0.2f;
        
        private CardPresenter _presenter;
        private Vector3 _originalScale = Vector3.one;
        private CanvasGroup _canvasGroup;
        private Transform _parentBeforeDrag;
        private int _siblingIndexBeforeDrag;
        private Canvas _canvas;
        
        public event Action<CardPresenter, Vector3> OnPutOnField;
        public event Action<CardPresenter, CardPresenter> OnPutOnCard;
        
        // New events for visual feedback coordination
        public event Action OnDragStart;
        public event Action<Vector2> OnDragUpdate;
        public event Action OnDragEnd;

        public CardPresenter Presenter => _presenter;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponentInParent<Canvas>();
        }

        public void Show(CardPresenter presenter)
        {
            _presenter = presenter;
            _presenter.Changed += HandleChanged;
            HandleChanged();
        }

        public void Hide()
        {
            if (_presenter != null)
            {
                _presenter.Changed -= HandleChanged;
                _presenter = null;
            }
        }
        
        public void SetGhostState(bool isGhost)
        {
            _canvasGroup.DOKill();
            float targetAlpha = isGhost ? 0f : 1f;
            _canvasGroup.DOFade(targetAlpha, 0.2f);
        }

        private void HandleChanged()
        {
            if (_presenter == null) return;

            if (_icon != null) _icon.sprite = _presenter.Icon;
            if (_nameText != null) _nameText.text = _presenter.Name;
            if (_typeIcon != null) _typeIcon.sprite = _presenter.TypeIcon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            transform.DOScale(_originalScale * _hoverScale, _scaleDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (eventData.dragging) return;
            transform.DOScale(_originalScale, _scaleDuration).SetEase(Ease.OutQuad);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _parentBeforeDrag = transform.parent;
            _siblingIndexBeforeDrag = transform.GetSiblingIndex();
            
            // Move to root canvas or a high-level container to render on top
            if (_canvas != null)
            {
                transform.SetParent(_canvas.transform, true);
            }
            
            _canvasGroup.blocksRaycasts = false;
            transform.DOScale(_originalScale, _scaleDuration); // Reset scale on drag
            
            OnDragStart?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_canvas == null) return;
            
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform, 
                eventData.position, 
                _canvas.worldCamera, 
                out pos);
            
            transform.localPosition = pos;
            
            OnDragUpdate?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnDragEnd?.Invoke();
            
            // Reset ghost state immediately
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;

            // Check for UI drop
            bool droppedOnUI = false;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.transform.IsChildOf(transform)) continue;

                CardWidget targetCard = result.gameObject.GetComponentInParent<CardWidget>();
                if (targetCard != null)
                {
                    OnPutOnCard?.Invoke(_presenter, targetCard.Presenter);
                    droppedOnUI = true;
                    break;
                }
                
                if (result.gameObject.GetComponent<RectTransform>() != null)
                {
                    droppedOnUI = true;
                }
            }

            if (!droppedOnUI)
            {
                // Check for 3D world drop
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    OnPutOnField?.Invoke(_presenter, hit.point);
                }
            }

            // Return to original parent
            transform.SetParent(_parentBeforeDrag);
            transform.SetSiblingIndex(_siblingIndexBeforeDrag);
            
            _canvasGroup.blocksRaycasts = true;
        }
    }
}