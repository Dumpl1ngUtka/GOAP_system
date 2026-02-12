using Services.GameCard;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI.View.Widgets
{
    public class CardWidget : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _typeIcon;
        [SerializeField] private Button _button;
        
        private CardPresenter _presenter;

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
    }
}