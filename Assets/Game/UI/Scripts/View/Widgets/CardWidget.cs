using DG.Tweening;
using Services.GameCard;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace UI.View.Widgets
{
    public class CardWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        public CardPresenter Presenter => _presenter;
        
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

        private void HandleChanged()
        {
            if (_presenter == null) return;

            if (_icon != null) _icon.sprite = _presenter.Icon;
            if (_nameText != null) _nameText.text = _presenter.Name;
            if (_typeIcon != null) _typeIcon.sprite = _presenter.TypeIcon;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOScale(_originalScale * _hoverScale, _scaleDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOScale(_originalScale, _scaleDuration).SetEase(Ease.OutQuad);
        }
    }
}