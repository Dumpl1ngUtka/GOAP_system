using System;

public interface IPresenter
{
    public event Action Changed;

    public void Start();
    public void Stop();
}