using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public abstract class PopupBase : UIBase
{
    [SerializeField] protected bool _isModal = true;
    [SerializeField] protected bool _closeOnOutsideClick = true;
    [SerializeField] protected bool _closeOnEscape = true;
    [SerializeField] protected int _priority = 0;
    [SerializeField] protected bool _pauseGameTime = false;

    [SerializeField] protected Image _backdrop;
    [SerializeField, Range(0f, 1f)] protected float _backdropAlpha = 0.85f;
    [SerializeField, Min(0f)] protected float _backdropFadeIn = 0.2f;
    [SerializeField, Min(0f)] protected float _backdropFadeOut = 0.2f;

    public int Priority => _priority;

    private bool _isTop; // флаг из стека

    public override void Initialize()
    {
        base.Initialize();
        if (_backdrop != null)
        {
            Color c = _backdrop.color;
            c.a = 0f;
            _backdrop.color = c;
            _backdrop.raycastTarget = _isModal;
        }
    }

    public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
    {
        if (_pauseGameTime) { TryPause(); }

        if (_backdrop != null && _isModal)
        {
            _backdrop.raycastTarget = true;
            _backdrop.DOFade(_backdropAlpha, _backdropFadeIn).SetUpdate(UseUnscaledTime);
        }

        base.Show(extraData, endCallback);
        PopupStack.Instance.Push(this);
    }

    public override void Hide(Action endCallback = null)
    {
        PopupStack.Instance.Pop(this);
        if (_backdrop != null)
        {
            _backdrop.DOFade(0f, _backdropFadeOut).SetUpdate(UseUnscaledTime)
                .OnComplete(() => { _backdrop.raycastTarget = false; });
        }

        base.Hide(() =>
        {
            if (_pauseGameTime) { TryUnpause(); }
            endCallback?.Invoke();
        });
    }

    public void Close(PopupCloseReason reason = PopupCloseReason.Programmatic)
    {
        if (!gameObject.activeSelf) { return; }
        Hide();
        OnClosed(reason);
    }

    protected virtual void OnClosed(PopupCloseReason reason) { }

    private void Update()
    {
        if (!_isTop) { return; } // только верхний реагирует на ввод
        // if (_closeOnEscape && Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Close(PopupCloseReason.Escape);
        // }
    }

    #region Стековые сигналы от PopupStack
    internal void SetIsTop(bool isTop)
    {
        _isTop = isTop;
        if (ContainerGroup != null)
        {
            ContainerGroup.interactable = isTop;
            ContainerGroup.blocksRaycasts = isTop;
        }
        if (_backdrop != null)
        {
            _backdrop.raycastTarget = _isModal && isTop;
        }
    }
    #endregion

    #region Outside click
    /// <summary>
    /// Навесь на Backdrop Button.OnClick через инспектор или вызови вручную.
    /// </summary>
    public void OnBackdropClicked()
    {
        if (_isTop && _closeOnOutsideClick)
        {
            Close(PopupCloseReason.OutsideClick);
        }
    }
    #endregion

    #region TimeScale
    private float _savedTimeScale = 1f;
    private void TryPause()
    {
        _savedTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void TryUnpause()
    {
        Time.timeScale = _savedTimeScale;
    }
    #endregion
}