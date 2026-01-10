// Assets/Modules/UI/Scripts/Runtime/Core/PresenterRegistry.cs
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PresenterRegistry", menuName = "Modules/UISystem/Presenter Registry")]
public sealed class PresenterRegistry : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public string InterfaceTypeName;
        public string ImplementationTypeName;
    }

    [SerializeField] private List<Entry> _entries = new List<Entry>();
    private Dictionary<Type, Type> _cache;

    public void BuildCache(bool verbose = false)
    {
        if (_cache != null) return;
        _cache = new Dictionary<Type, Type>();
        int ok = 0, bad = 0;
        for (int i = 0; i < _entries.Count; i++)
        {
            var e = _entries[i];
            var iface = Type.GetType(e.InterfaceTypeName);
            var impl  = Type.GetType(e.ImplementationTypeName);
            if (iface == null || impl == null)
            {
                bad++;
                //if (verbose) Debug.LogError($"Registry type resolve error: {e.InterfaceTypeName} -> {e.ImplementationTypeName}");
                continue;
            }
            _cache[iface] = impl;
            ok++;
            //if (verbose) Debug.Log($"Registry OK: {iface.FullName} -> {impl.FullName}");
        }
        if (verbose) Debug.Log($"Registry cache built: ok={ok}, bad={bad}");
    }

    public Type GetImplementation(Type interfaceType)
    {
        if (_cache == null) BuildCache();
        if (_cache != null && _cache.TryGetValue(interfaceType, out var impl))
            return impl;
        Debug.LogError("PresenterRegistry: no implementation for " + interfaceType?.FullName);
        return null;
    }

    // отладочная печать
    public void Dump()
    {
        BuildCache();
        // if (_cache == null) { Debug.LogWarning("Registry cache is null"); return; }
        // foreach (var kv in _cache)
        //     Debug.Log($"RegistryEntry: {kv.Key.FullName} -> {kv.Value.FullName}");
    }
}
