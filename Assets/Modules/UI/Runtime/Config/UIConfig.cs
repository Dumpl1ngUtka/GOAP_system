using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UIConfig", menuName = "Static Data/UIConfig")]
public class UIConfig : ScriptableObject
{
    [field: SerializeField] public UIRoot UiRoot { get; private set; }

    [SerializeField] private List<PopupData> _popupDatas;
    [SerializeField] private List<WidgetData> _widgetDatas;
    [SerializeField] private List<WindowData> _windowDatas;
    
    public GameObject GetUIBase(string id, UIType type)
    {
        switch (type)
        {
            case UIType.Window:
                return GetPrefabFromList(_windowDatas, id);
            case UIType.Widget:
                return GetPrefabFromList(_widgetDatas, id);
            case UIType.Popup:
                return GetPrefabFromList(_popupDatas, id);
            default:
                Debug.LogError($"Unknown UIType: {type}");
                return null;
        }
    }
    
    private GameObject GetPrefabFromList<T>(List<T> list, string id) where T : UIData
    {
        T data = list.FirstOrDefault(a => a.Id == id);
        if (data == null)
        {
            Debug.LogWarning($"{typeof(T).Name} with id '{id}' not found");
            return null;
        }

        if (data.Prefab == null)
        {
            Debug.LogError($"{typeof(T).Name} prefab for id '{id}' is missing");
            return null;
        }

        return data.Prefab;
    }
}

public enum UIType
{
    Window = 0,
    Widget = 1,
    Popup = 2,
    Empty = 3
}

public abstract class UIData
{
    public string Id;
    public GameObject Prefab;
}