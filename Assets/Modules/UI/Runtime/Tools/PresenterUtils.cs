using System;
using System.Collections.Generic;
using UnityEngine;

public static class PresenterUtils
{
    // 🔧 [CHANGED]
    public static bool Setup<TPresenter>(
        Dictionary<string, object> extraData,
        string key,
        Action subscribeAction,
        ref TPresenter presenterField)
        where TPresenter : class, IPresenter
    {
        if (extraData != null && extraData.TryGetValue(key, out object obj))
            presenterField = obj as TPresenter;

        if (presenterField == null)
            return false;
        //Debug.Log(key + " - FFFFFFFFFFFFFFFFF");
        presenterField.Start();
        presenterField.Changed += subscribeAction;
        return true;
    }

    // 🔧 [CHANGED]
    public static bool Teardown<TPresenter>(
        ref TPresenter presenterField,
        Action unsubsrcibeAction,
        Action startAction = null)
        where TPresenter : class, IPresenter
    {
        if (presenterField == null)
            return false;

        startAction?.Invoke();
        presenterField.Changed -= unsubsrcibeAction;
        presenterField.Stop();
        presenterField = null;
        return true;
    }
}