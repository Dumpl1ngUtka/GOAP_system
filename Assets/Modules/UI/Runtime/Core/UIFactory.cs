using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public sealed class UIFactory
{
    private readonly UIConfig _uiConfig;
    private readonly UIRoot _uiRoot;
    private readonly DiContainer _diContainer;

    public UIFactory(UIConfig uiConfig, UIRoot uiRoot, DiContainer diContainer)
    {
        _uiConfig = uiConfig;
        _uiRoot = uiRoot;
        _diContainer = diContainer;
    }

    private IPresenter _presenterBase;
    
    //[Button]
    // public UIBase Create(string key, IPresenter presenter)
    // {
    //     Debug.LogError($"Creating UI {key}");
    //     UIBase uiBase = _diContainer.InstantiatePrefabForComponent<UIBase>
    //     (
    //         _uiConfig.GetUIBase(GlobalKeys.UI.Window.GameLoopWindow, UIType.Window),
    //         _uiRoot.transform
    //     );
    //
    //     Dictionary<string, object> extraData = new();
    //     extraData.TryAdd(key, presenter);
    //     
    //     uiBase.Show(extraData: extraData);
    //     Debug.Log($"Creating window with key {key}");
    //     return uiBase;
    // }
    
    
    //public TWindow Create<TWindow>(string key, IPresenter presenter)
    public TWindow Create<TWindow>(string key, Transform parent = null)
        where TWindow : UIBase
    {
        Type type = typeof(TWindow);
        UIType uiType = UIType.Empty;
        
        //Debug.Log(type.ToString() + " - WTF");
        if (typeof(WidgetBase).IsAssignableFrom(type))
        {
            //Debug.Log("Creating UI Widget");
            uiType = UIType.Widget;
        }
        else if (typeof(PopupBase).IsAssignableFrom(type))
        {
            //Debug.Log("Creating UI Popup");
            uiType = UIType.Popup;
        }
        else if (typeof(WindowBase).IsAssignableFrom(type))
        {
            //Debug.Log("Creating UI Window");
            uiType = UIType.Window;
        }
        
        GameObject prefab = _uiConfig.GetUIBase(key, uiType);
        Transform targetParent = parent != null ? parent : _uiRoot.transform;
        
        TWindow window = _diContainer.InstantiatePrefabForComponent<TWindow>(
            prefab,
            targetParent
        );
        
        return window;
    }
}