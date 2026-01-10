#nullable enable

using System;

public struct ChildPresenterSpecification
{
    public readonly string Id;
    public readonly Type InterfaceType;

    public ChildPresenterSpecification(string id, Type interfaceType)
    {
        Id = id;
        InterfaceType = interfaceType;
    }
}
