using System;

// помечаем классы-имплементации, к какому интерфейсу они относятся
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PresenterImplAttribute : Attribute
{
    public Type InterfaceType { get; }

    public PresenterImplAttribute(Type interfaceType)
    {
        InterfaceType = interfaceType;
    }
}