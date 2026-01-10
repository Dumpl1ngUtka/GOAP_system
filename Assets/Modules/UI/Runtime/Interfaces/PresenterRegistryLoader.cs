using UnityEngine;

public static class PresenterRegistryLoader
{
    private static PresenterRegistry _cached;

    public static PresenterRegistry Load()
    {
        if (_cached != null)
            return _cached;
        
        _cached = Resources.Load<PresenterRegistry>("PresenterRegistry");
        if (_cached == null)
            Debug.LogError("PresenterRegistryLoader: Resources/PresenterRegistry.asset not found");
        return _cached;
    }
}