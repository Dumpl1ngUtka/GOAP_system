using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// Простой менеджер стека попапов по приоритету.
/// Гарантирует, что ввод получает только верхний.
/// </summary>
public sealed class PopupStack : MonoBehaviour
{
    private static PopupStack _instance;

    [Obsolete("Obsolete")]
    public static PopupStack Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PopupStack>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PopupStack");
                    _instance = go.AddComponent<PopupStack>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    [ReadOnly] private List<PopupBase> _stack = new List<PopupBase>();

    public void Push(PopupBase popup)
    {
        if (_stack.Contains(popup))
        {
            return;
        }

        _stack.Add(popup);
        Resort();
        RefreshTop();
    }

    public void Pop(PopupBase popup)
    {
        int index = _stack.IndexOf(popup);
        if (index >= 0)
        {
            _stack.RemoveAt(index);
            RefreshTop();
        }
    }

    private void Resort()
    {
        _stack.Sort((a, b) => a.Priority.CompareTo(b.Priority)); // низкий вниз, высокий вверх
    }

    private void RefreshTop()
    {
        int last = _stack.Count - 1;
        for (int i = 0; i < _stack.Count; i++)
        {
            bool isTop = i == last;
            _stack[i].SetIsTop(isTop);
        }
    }
}