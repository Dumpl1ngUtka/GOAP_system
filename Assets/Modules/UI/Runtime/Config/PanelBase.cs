using System;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour, IPositionUpdatable
{
    public event Action OnPositionUpdated;

    public float DefaultYPosition { get; private set; }

    public virtual void Show() =>
        gameObject.SetActive(true);

    public virtual void Hide() =>
        Destroy(gameObject);

    public void UpdateDefaultYPosition(float defaultPosition)
    {
        DefaultYPosition = defaultPosition;
        OnPositionUpdated?.Invoke();
    }
}