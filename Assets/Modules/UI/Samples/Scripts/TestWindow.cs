using System;
using System.Collections.Generic;
using UnityEngine;

public class TestWindow : WindowBase, IChildPresenterRequest
{
    private ITestPresenter _presenter;
    
    public IEnumerable<ChildPresenterSpecification> GetChildPresenters()
    {
        return new List<ChildPresenterSpecification>();
    }
    
    public override void Show(Dictionary<string, object> extraData = null, Action endCallback = null)
    {
        base.Show(extraData, endCallback);
    }

    public override void Hide(Action endCallback = null)
    {
        base.Hide(endCallback);
    }
}
