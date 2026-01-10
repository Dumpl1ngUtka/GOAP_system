using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum PopupCloseReason
{
    Unknown = 0,
    Programmatic = 1,
    OutsideClick = 2,
    Escape = 3
}

/// <summary>
/// База для НЕ-модальных элементов/виджетов. Без бэкдропа и стека.
/// Управляет только контейнером (_containerGroup).
/// </summary>
///
/// 
public abstract class UIBase : MonoBehaviour
{
    public string Id => _id;

    [SerializeField] private string _id;
    [SerializeField] protected CanvasGroup ContainerGroup;

    [SerializeField]
    protected bool UseUnscaledTime = false;

    [SerializeField, Min(0f)]
    protected float FadeIn = 0.2f;

    [SerializeField, Min(0f)]
    protected float FadeOut = 0.2f;

    [SerializeField]
    protected bool UseScale = false;

    [SerializeField]
    protected Vector3 ScaleFrom = new Vector3(0.95f, 0.95f, 1f);

    [SerializeField, Min(0f)]
    protected float ScaleIn = 0.2f;

    [SerializeField, Min(0f)]
    protected float ScaleOut = 0.15f;

    [SerializeField]
    protected Ease EaseIn = Ease.OutQuad;

    [SerializeField]
    protected Ease EaseOut = Ease.InQuad;

    protected Sequence Sequence;

    public virtual void Subscribe()
    {
    }

    public virtual void Unsubscribe()
    {
    }

    public virtual void Initialize()
    {
        if (ContainerGroup != null)
        {
            ContainerGroup.alpha = 0f;
            ContainerGroup.interactable = false;
            ContainerGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
    }

    public virtual void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
    {
        Subscribe();
        gameObject.SetActive(true);

        if (Sequence != null)
        {
            Sequence.Kill(false);
        }

        Sequence = DOTween.Sequence().SetUpdate(UseUnscaledTime);

        if (ContainerGroup != null)
        {
            ContainerGroup.alpha = 0f;
            ContainerGroup.interactable = false;
            ContainerGroup.blocksRaycasts = false;
        }

        if (UseScale)
        {
            Transform t = transform;
            t.localScale = ScaleFrom;
            Sequence.Join(t.DOScale(Vector3.one, ScaleIn).SetEase(EaseIn));
        }

        if (ContainerGroup != null)
            Sequence.Join(ContainerGroup.DOFade(1f, FadeIn));

        Sequence.OnComplete(() =>
        {
            if (ContainerGroup != null)
            {
                ContainerGroup.interactable = true;
                ContainerGroup.blocksRaycasts = true;
            }

            endCallback?.Invoke();
        });
    }

    public virtual void Hide(Action endCallback = null)
    {
        Unsubscribe();

        if (Sequence != null)
        {
            Sequence.Kill(false);
        }

        Sequence = DOTween.Sequence().SetUpdate(UseUnscaledTime);

        if (UseScale)
        {
            Transform t = transform;
            Sequence.Join(t.DOScale(ScaleFrom, ScaleOut).SetEase(EaseOut));
        }

        if (ContainerGroup != null)
        {
            ContainerGroup.interactable = false;
            ContainerGroup.blocksRaycasts = false;
            Sequence.Join(ContainerGroup.DOFade(0f, FadeOut));
        }

        Sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            endCallback?.Invoke();
        });
    }

    public virtual void FastShow(Action endCallback = null)
    {
        Subscribe();
        gameObject.SetActive(true);
        if (ContainerGroup != null)
        {
            ContainerGroup.alpha = 1f;
            ContainerGroup.interactable = true;
            ContainerGroup.blocksRaycasts = true;
        }

        transform.localScale = Vector3.one;
        endCallback?.Invoke();
    }

    public virtual void FastHide(Action endCallback = null)
    {
        Unsubscribe();
        if (ContainerGroup != null)
        {
            ContainerGroup.alpha = 0f;
            ContainerGroup.interactable = false;
            ContainerGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
        endCallback?.Invoke();
    }

    protected virtual void OnDestroy()
    {
        if (Sequence != null)
        {
            Sequence.Kill(false);
        }
    }
}