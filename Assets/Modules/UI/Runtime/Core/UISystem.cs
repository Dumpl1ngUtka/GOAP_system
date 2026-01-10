// UISystem.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

[Serializable]
public sealed class UISystem
{
    // 🔧 [NEW] Рабочие структуры, используемые ТОЛЬКО логикой
    private readonly Dictionary<string, UIBase> _runtimeViews = new();                       // 🔧 [NEW]
    private readonly Dictionary<string, List<IPresenter>> _runtimeChildPresenters = new();   // 🔧 [NEW]

    // 🔧 [NEW] ДЕБАГ-ЗЕРКАЛА. Видны в рантайме. Не участвуют в логике НИГДЕ.
    private Dictionary<string, IPresenter> _presenters = new();            // 🔧 [NEW]
    private Dictionary<string, UIBase> _views = new();                     // 🔧 [NEW]
    private Dictionary<string, List<IPresenter>> _childPresenters = new(); // 🔧 [NEW]

    private readonly IInstantiator _instantiator;
    private readonly UIFactory _factory;
    private PresenterRegistry _presenterRegistry;

    // Опциональный провайдер дополнительных аргументов конструктора презентера
    private Func<string, Type, object[]> _presenterArgsProvider;

    public UISystem(IInstantiator instantiator, UIFactory factory)
    {
        _instantiator = instantiator;
        _factory = factory;

        _presenterRegistry = PresenterRegistryLoader.Load(); // Resources/PresenterRegistry.asset
        Debug_ValidateRegistry(_presenterRegistry);
    }

    public bool HasOpenPopups() => _runtimeViews.Values.OfType<PopupBase>().Any();

    // 🔧 [NEW]
    public void SetArgsProvider(Func<string, Type, object[]> provider)
    {
        _presenterArgsProvider = provider;
    }

    // ✅ [CHANGED] Всегда создаём НОВЫЙ презентер. Дебаг-словарь только отображает.
    public void Start<TPresenter, TWindow>(string id)
        where TPresenter : class, IPresenter
        where TWindow : UIBase
    {
        IPresenter presenter = CreatePresenter<TPresenter>(id, null);   // всегда новый
        ShowView<TWindow>(id, presenter, null);
    }

    // ✅ [CHANGED]
    public void Start<TPresenter, TWindow>(string id, Dictionary<string, object> extraData)
        where TPresenter : class, IPresenter
        where TWindow : UIBase
    {
        IPresenter presenter = CreatePresenter<TPresenter>(id, null);   // всегда новый
        ShowView<TWindow>(id, presenter, extraData);
    }

    // ✅ [CHANGED]
    public void Start<TPresenter, TWindow>(string id, object[] presenterArgs)
        where TPresenter : class, IPresenter
        where TWindow : UIBase
    {
        IPresenter presenter = CreatePresenter<TPresenter>(id, presenterArgs); // всегда новый
        ShowView<TWindow>(id, presenter, null);
    }

    // ✅ [CHANGED] Никогда не кэшируем презентер для логики. Только отражаем в _presenters для дебага.
    private IPresenter CreatePresenter<TPresenter>(string id, object[] explicitArgs)
        where TPresenter : class, IPresenter
    {
        object[] provided = explicitArgs ?? _presenterArgsProvider?.Invoke(id, typeof(TPresenter));
        object[] finalArgs = SanitizeArgs(typeof(TPresenter), provided);

        if (provided != null && provided.Length > 0 && (finalArgs == null || finalArgs.Length == 0))
            Debug.LogWarning($"UISystem: extraArgs для {typeof(TPresenter).Name} отфильтрованы. Проверь сигнатуру конструктора.");

        IPresenter presenter = (finalArgs != null && finalArgs.Length > 0)
            ? _instantiator.Instantiate<TPresenter>(finalArgs)
            : _instantiator.Instantiate<TPresenter>();

        // 🔧 [NEW] ДЕБАГ-ОТРАЖЕНИЕ
        _presenters[id] = presenter;

        return presenter;
    }

    // ✅ [CHANGED] Вся логика — через _runtimeViews/_runtimeChildPresenters. Дебаг-словарь _views/_childPresenters только зеркалит.
    private void ShowView<TWindow>(string id, IPresenter presenter, Dictionary<string, object> incomingExtraData)
        where TWindow : UIBase
    {
        // ЛОГИКА: создать или взять вью из _runtimeViews
        if (!_runtimeViews.TryGetValue(id, out UIBase uiBase))
        {
            uiBase = _factory.Create<TWindow>(id);
            _runtimeViews[id] = uiBase;
        }

        // ДЕБАГ-ОТРАЖЕНИЕ для вью
        _views[id] = uiBase; // 🔧 [NEW]

        Dictionary<string, object> extraData = incomingExtraData != null
            ? new Dictionary<string, object>(incomingExtraData)
            : new Dictionary<string, object>();

        if (!extraData.ContainsKey(id))
            extraData.Add(id, presenter);

        IChildPresenterRequest request = uiBase as IChildPresenterRequest;
        if (request != null)
        {
            List<IPresenter> runtimeChildren = new List<IPresenter>();
            IEnumerable<ChildPresenterSpecification> specifications = request.GetChildPresenters();

            foreach (ChildPresenterSpecification spec in specifications)
            {
                IPresenter child = ResolveByInterface(spec.InterfaceType);
                runtimeChildren.Add(child);

                if (!extraData.ContainsKey(spec.Id))
                    extraData.Add(spec.Id, child);
            }

            // ЛОГИКА: сохранить детей
            _runtimeChildPresenters[id] = runtimeChildren;

            // ДЕБАГ-ОТРАЖЕНИЕ для детей
            _childPresenters[id] = new List<IPresenter>(runtimeChildren); // 🔧 [NEW]
        }
        else
        {
            // если окно не просит детей — очищаем отражение
            _runtimeChildPresenters.Remove(id);
            _childPresenters.Remove(id); // 🔧 [NEW]
        }

        uiBase.Show(extraData);
    }

    private IPresenter ResolveByInterface(Type interfaceType)
    {
        Type impl = _presenterRegistry.GetImplementation(interfaceType);
        if (impl == null)
            throw new Exception($"UISystem: implementation not found for {interfaceType?.FullName}");

        IPresenter presenter = (IPresenter)_instantiator.Instantiate(impl);
        if (!interfaceType.IsInstanceOfType(presenter))
            Debug.LogError($"UISystem: created {presenter.GetType().FullName} does not implement {interfaceType.FullName}");
        return presenter;
    }

    // Можно включать для диагностики реестра
    private void Debug_ValidateRegistry(PresenterRegistry reg)
    {
        if (reg == null)
        {
            Debug.LogError("UISystem: PresenterRegistry missing");
            return;
        }
        reg.BuildCache(verbose: true);
        reg.Dump();
    }
    
    public void Stop(string id)
    {
        HideView(id, destroyGameObject: true);

        // ЛОГИКА: убрать детей
        _runtimeChildPresenters.Remove(id);

        // ДЕБАГ-ОЧИСТКА: презентер главного окна и дети
        _presenters.Remove(id);          // 🔧 [NEW]
        _childPresenters.Remove(id);     // 🔧 [NEW]
        _views.Remove(id);               // 🔧 [NEW]
    }

    private void HideView(string id, bool destroyGameObject)
    {
        if (_runtimeViews.TryGetValue(id, out UIBase uiBase))
        {
            uiBase.Hide();

            if (destroyGameObject && uiBase != null)
                Object.Destroy(uiBase.gameObject);

            _runtimeViews.Remove(id);
        }
    }

    // Фильтрация лишних аргументов для Zenject
    private object[] SanitizeArgs(Type type, object[] args)
    {
        if (args == null || args.Length == 0)
            return Array.Empty<object>();

        List<object> result = new List<object>();
        ParameterInfo[] allParams = type
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
            .SelectMany(c => c.GetParameters())
            .ToArray();

        for (int i = 0; i < args.Length; i++)
        {
            object obj = args[i];
            if (obj == null)
                continue;

            Type argumentType = obj.GetType();
            bool ok = false;
            for (int p = 0; p < allParams.Length; p++)
            {
                if (allParams[p].ParameterType.IsAssignableFrom(argumentType))
                {
                    ok = true;
                    break;
                }
            }

            if (ok)
                result.Add(obj);
        }

        return result.ToArray();
    }
}


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using Zenject;
// using Object = UnityEngine.Object;
//
// [Serializable]
// public sealed class UISystem
// {
//     [ShowInInspector]
//     private Dictionary<string, IPresenter> _presenters = new();
//     
//     [ShowInInspector]
//     private Dictionary<string, UIBase> _views = new();
//     
//     [ShowInInspector]
//     private Dictionary<string, List<IPresenter>> _childPresenters = new();
//     
//     private readonly IInstantiator _instantiator;
//     private readonly UIFactory _factory;
//     private PresenterRegistry _presenterRegistry;
//     private Func<string, Type, object[]> _presenterArgsProvider; 
//
//     public UISystem(IInstantiator instantiator, UIFactory factory)
//     {
//         _instantiator = instantiator;
//         _factory = factory;
//         
//         _presenterRegistry = PresenterRegistryLoader.Load(); // Resources/PresenterRegistry.asset
//         Debug_ValidateRegistry(_presenterRegistry);  
//     }
//     
//     // 🔧 [NEW] Регистрация провайдера
//     // public void SetArgsProvider(Func<string, Type, object[]> provider)
//     // {
//     //     _presenterArgsProvider = provider;
//     // }
//     
//     public void Start<TPresenter, TWindow>(string id) 
//         where TPresenter : class, IPresenter
//         where TWindow : UIBase
//     {
//         if (!_presenters.TryGetValue(id, out IPresenter presenter))
//         {
//             presenter = CreatePresenter<TPresenter>(id);
//         }
//         
//         ShowView<TWindow>(id, presenter, null); // ✅ [CHANGED] прокидываем null, перегрузка собирает extraData
//     }
//
//     // 🔧 [NEW] Перегрузка с опциональным extraData
//     public void Start<TPresenter, TWindow>(string id, Dictionary<string, object> extraData)
//         where TPresenter : class, IPresenter
//         where TWindow : UIBase
//     {
//         if (!_presenters.TryGetValue(id, out IPresenter presenter))
//         {
//             presenter = CreatePresenter<TPresenter>(id);
//         }
//
//         ShowView<TWindow>(id, presenter, extraData);
//     }
//     
//     public void Start<TPresenter, TWindow>(string id, object[] presenterArgs)
//         where TPresenter : class, IPresenter
//         where TWindow : UIBase
//     {
//         if (!_presenters.TryGetValue(id, out IPresenter presenter))
//         {
//             presenter = CreatePresenter<TPresenter>(id, presenterArgs);
//         }
//
//         ShowView<TWindow>(id, presenter, null); 
//     }
//     
//     private IPresenter CreatePresenter<TPresenter>(string id, object[] args) where TPresenter : class, IPresenter
//     {
//         if (_presenters.ContainsKey(id))
//         {
//             Debug.Log("Создаешь дубликат презентера!!!");
//             return _presenters[id];
//         }
//
//         // 🔧 [NEW] Получаем аргументы
//         object[] provided = args ?? _presenterArgsProvider?.Invoke(id, typeof(TPresenter));
//         object[] finalArgs = SanitizeArgs(typeof(TPresenter), provided);
//
//         IPresenter presenter = finalArgs != null && finalArgs.Length > 0
//             ? _instantiator.Instantiate<TPresenter>(finalArgs)
//             : _instantiator.Instantiate<TPresenter>();
//
//         _presenters.Add(id, presenter);
//         return presenter;
//     }
//     
//     private IPresenter CreatePresenter<TPresenter>(string id) where TPresenter : class, IPresenter
//     {
//         if (_presenters.ContainsKey(id))
//         {
//             Debug.Log("Создаешь дубликат презентера!!!");
//             return _presenters[id];
//         }
//
//         IPresenter presenter = _instantiator.Instantiate<TPresenter>();
//         _presenters.Add(id, presenter);
//         
//         return presenter;
//     }
//     
//     // ✅ [CHANGED] добавлен параметр incomingExtraData
//     private void ShowView<TWindow>(string id, IPresenter presenter, Dictionary<string, object> incomingExtraData) where TWindow : UIBase
//     {
//         if (!_views.TryGetValue(id, out UIBase uiBase))
//         {
//             uiBase = _factory.Create<TWindow>(id);
//             _views.TryAdd(id, uiBase);
//         }
//
//         // 🔧 [NEW] Собираем итоговый extraData: пользовательские ключи остаются, системные добавляем, не перезаписывая
//         Dictionary<string, object> extraData = incomingExtraData != null
//             ? new Dictionary<string, object>(incomingExtraData)
//             : new Dictionary<string, object>();
//
//         // 🔧 [NEW] Гарантируем ссылку на главный презентер по ключу окна, если пользователь её не задал
//         if (!extraData.ContainsKey(id))
//         {
//             extraData.Add(id, presenter);
//         }
//         
//         IChildPresenterRequest request = uiBase as IChildPresenterRequest;
//         if (request != null)
//         {
//             List<IPresenter> childPresenters = new List<IPresenter>();
//             IEnumerable<ChildPresenterSpecification> specifications = request.GetChildPresenters();
//         
//             foreach (ChildPresenterSpecification childSpecification in specifications)
//             {
//                 // 🔧 [NEW] Резолв дочерних по интерфейсу
//                 IPresenter child = ResolveByInterface(childSpecification.InterfaceType);
//                 childPresenters.Add(child);
//
//                 // 🔧 [NEW] Добавляем детей в extraData, не перезаписывая пользовательские ключи
//                 if (!extraData.ContainsKey(childSpecification.Id))
//                 {
//                     extraData.Add(childSpecification.Id, child);
//                 }
//             }
//
//             _childPresenters[id] = childPresenters;
//         }
//
//         uiBase.Show(extraData: extraData);
//     }
//
//     // вызывается из ShowView, когда окно просит детей через IChildPresentersRequest
//     private IPresenter ResolveByInterface(Type interfaceType)
//     {
//         // ✅ [CHANGED] без var
//         Type impl = _presenterRegistry.GetImplementation(interfaceType);
//         if (impl == null)
//             throw new Exception($"UISystem: implementation not found for {interfaceType?.FullName}");
//
//         IPresenter presenter = (IPresenter)_instantiator.Instantiate(impl);
//         if (presenter == null)
//             throw new Exception($"UISystem: DI failed for {impl.FullName}");
//         if (!interfaceType.IsInstanceOfType(presenter))
//             Debug.LogError($"UISystem: created {presenter.GetType().FullName} does not implement {interfaceType.FullName}");
//         return presenter;
//     }
//
//     // включай временно для диагностики реестра
//     private void Debug_ValidateRegistry(PresenterRegistry reg)
//     {
//         if (reg == null) { Debug.LogError("UISystem: PresenterRegistry missing"); return; }
//         reg.BuildCache(verbose: true);
//         reg.Dump();
//     }
//     
//     [Button]
//     public void Stop(string id)
//     {
//         HideView(id, destroyGameObject: true);
//         DestroyPresenter(id);
//     }
//
//     private void DestroyPresenter(string id)
//     {
//         if (_presenters.Remove(id, out IPresenter presenter))
//         {
//             // можно вызвать Dispose тут, если нужно
//             Debug.Log("Destroyed Presenter - " + id);
//         }
//         else
//         {
//             Debug.LogWarning("No Presenter for id: " + id);
//         }
//
//         // 🔧 [NEW] Удаляем и дочерние презентеры, если были
//         if (_childPresenters.Remove(id, out List<IPresenter> children))
//         {
//             for (int i = 0; i < children.Count; i++)
//             {
//                 IPresenter child = children[i];
//                 // при необходимости: (child as IDisposable)?.Dispose();
//             }
//         }
//     }
//
//     private void DestroyChildPresenter()
//     {
//         // зарезервировано под точечное удаление детей по ключу
//     }
//
//     private void HideView(string id, bool destroyGameObject)
//     {
//         if (_views.TryGetValue(id, out UIBase uiBase))
//         {
//             uiBase.Hide();
//
//             if (destroyGameObject)
//             {
//                 Object.Destroy(uiBase.gameObject);
//                 //Debug.Log("Destroyed View - " + id);
//             }
//
//             _views.Remove(id);
//         }
//         else
//         {
//             //Debug.LogWarning("No View for id: " + id);
//         }
//     }
//     
//     // 🔧 [NEW] Метод фильтрации лишних аргументов для Zenject
//     private object[] SanitizeArgs(Type type, object[] args)
//     {
//         if (args == null || args.Length == 0)
//             return Array.Empty<object>();
//
//         List<object> result = new List<object>();
//         ParameterInfo[] allParams = type
//             .GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
//             .SelectMany(c => c.GetParameters())
//             .ToArray();
//
//         for (int i = 0; i < args.Length; i++)
//         {
//             object obj = args[i];
//             if (obj == null)
//                 continue;
//
//             Type t = obj.GetType();
//             bool ok = false;
//             for (int p = 0; p < allParams.Length; p++)
//             {
//                 if (allParams[p].ParameterType.IsAssignableFrom(t))
//                 {
//                     ok = true;
//                     break;
//                 }
//             }
//
//             if (ok)
//                 result.Add(obj);
//         }
//
//         return result.ToArray();
//     }
// }