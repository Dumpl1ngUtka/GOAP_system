using System;
using System.Collections.Generic;
using UnityEngine;

public class TestPopup : PopupBase
{
    private ITestPresenter _presenter;
    
    public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
    {
        base.Show(extraData, endCallback);

        Debug.Log("Show TestPopup");
        if (PresenterUtils.Setup(extraData,
                key: Id,
                subscribeAction: HandleChanged,
                presenterField: ref _presenter))
        {
            
        }
    }

    public override void Hide(Action endCallback = null)
    {
        PresenterUtils.Teardown(ref _presenter, HandleChanged, () =>
        {

        });
        Debug.Log("Hide TestPopup");
        base.Hide(endCallback);
    }

    private void HandleChanged()
    {
        
    }
}