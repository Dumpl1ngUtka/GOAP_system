// Assets/Modules/UI/Editor/PresenterRegistryBuilder.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.TypeCache;
using UnityEngine;

public static class PresenterRegistryBuilder
{
    private const bool VERBOSE = true; // включи для подробного лога
    private const string AssetPath = "Assets/Modules/UI/Resources/PresenterRegistry.asset";

    [MenuItem("Tools/UI/Build Presenter Registry")]
    public static void Build()
    {
        var entries = new List<PresenterRegistry.Entry>();
        int scanned = 0, withAttr = 0, valid = 0, skipped = 0;

        var types = TypeCache.GetTypesWithAttribute<PresenterImplAttribute>();
        foreach (var t in types)
        {
            scanned++;
            if (t == null) { skipped++; if (VERBOSE) Debug.LogWarning("Null type"); continue; }
            if (t.IsAbstract || t.IsInterface) { skipped++; if (VERBOSE) Debug.LogWarning($"Skip abstract/interface {t.FullName}"); continue; }
            if (!typeof(IPresenter).IsAssignableFrom(t)) { skipped++; if (VERBOSE) Debug.LogWarning($"Skip {t.FullName}: not IPresenter"); continue; }

            var attrs = (PresenterImplAttribute[])t.GetCustomAttributes(typeof(PresenterImplAttribute), false);
            withAttr += attrs.Length;

            foreach (var a in attrs)
            {
                var iface = a.InterfaceType;
                if (iface == null) { skipped++; if (VERBOSE) Debug.LogWarning($"Skip {t.FullName}: null InterfaceType"); continue; }
                if (!iface.IsInterface) { skipped++; if (VERBOSE) Debug.LogWarning($"Skip {t.FullName}: {iface.FullName} not interface"); continue; }
                if (!typeof(IPresenter).IsAssignableFrom(iface)) { skipped++; if (VERBOSE) Debug.LogWarning($"Skip {t.FullName}: {iface.FullName} not IPresenter"); continue; }

                entries.Add(new PresenterRegistry.Entry
                {
                    InterfaceTypeName = iface.AssemblyQualifiedName,
                    ImplementationTypeName = t.AssemblyQualifiedName
                });
                valid++;
                if (VERBOSE) Debug.Log($"OK: {iface.FullName} -> {t.FullName}");
            }
        }

        // создать/перезаписать ассет
        var asset = ScriptableObject.CreateInstance<PresenterRegistry>();
        var so = new SerializedObject(asset);
        var list = so.FindProperty("_entries");
        list.arraySize = entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            var e = list.GetArrayElementAtIndex(i);
            e.FindPropertyRelative("InterfaceTypeName").stringValue = entries[i].InterfaceTypeName;
            e.FindPropertyRelative("ImplementationTypeName").stringValue = entries[i].ImplementationTypeName;
        }
        so.ApplyModifiedProperties();

        AssetDatabase.CreateAsset(asset, AssetPath);
        AssetDatabase.SaveAssets();

        Debug.Log($"PresenterRegistry built: {AssetPath} (scannedTypes={scanned}, withAttr={withAttr}, valid={valid}, skipped={skipped})");
        if (entries.Count == 0)
            Debug.LogWarning("PresenterRegistry is empty. Проверь атрибуты, asmdef зависимости, и что классы public.");
    }

    [MenuItem("Tools/UI/Validate Presenter Setup")]
    public static void ValidateSetup()
    {
        // 1) найдём дубликаты интерфейсов
        var map = new Dictionary<string, string>();
        var dups = new List<string>();
        var types = TypeCache.GetTypesWithAttribute<PresenterImplAttribute>();
        foreach (var t in types)
        {
            var attrs = (PresenterImplAttribute[])t.GetCustomAttributes(typeof(PresenterImplAttribute), false);
            foreach (var a in attrs)
            {
                var iface = a.InterfaceType;
                if (iface == null) continue;
                var key = iface.AssemblyQualifiedName;
                if (map.TryGetValue(key, out var prev))
                {
                    dups.Add($"{iface.FullName}\n  -> {prev}\n  -> {t.FullName}");
                }
                else map[key] = t.FullName;
            }
        }

        if (dups.Count > 0)
        {
            Debug.LogError("Duplicate presenter implementations:\n" + string.Join("\n\n", dups));
        }
        else Debug.Log("Validate: no duplicate interface implementations found.");

        // 2) проверим наличие ресурса
        var reg = Resources.Load<PresenterRegistry>("PresenterRegistry");
        Debug.Log(reg != null ? "Validate: Resources/PresenterRegistry.asset found." : "Validate: PresenterRegistry.asset NOT found in Resources/.");
    }
}
#endif
