using System;

[PresenterImpl(typeof(ITestPresenter))]
public class TestPresenter : ITestPresenter
{
    public event Action Changed;

    void IPresenter.Start()
    {
        
    }

    void IPresenter.Stop()
    {
        
    }
}