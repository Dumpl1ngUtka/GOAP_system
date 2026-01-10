using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class UIPointerListener :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler,
    IPointerMoveHandler
{
    public event Action<PointerEventData> PointerDown;
    public event Action<PointerEventData> PointerUp;
    public event Action<PointerEventData> PointerExit;
    public event Action<PointerEventData> PointerMove;
    
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        PointerDown?.Invoke(eventData);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        PointerUp?.Invoke(eventData);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        PointerExit?.Invoke(eventData);
    }

    void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
    {
        Debug.Log("OnPointerMove");
        PointerMove?.Invoke(eventData);
    }
}