# Aspid.MVVM

> **Scope**: UI development using Aspid.MVVM framework — source-generated ViewModels, data binding, commands, observable collections, and Zenject integration.
> **Load when**: designing UI windows, creating ViewModels, binding data to uGUI elements, implementing UI logic, wiring MVVM components with Zenject.

---

## Core Principles

- **ViewModel as Source of Truth**: The ViewModel must contain all state and logic for the View. It should not reference Unity components directly (e.g., `GameObject`, `Transform`, `Image`).
- **Source Generation**: Aspid.MVVM uses C# Source Generators. ViewModels MUST be `partial` and marked with the `[ViewModel]` attribute.
- **Presenter Integration**: Presenters act as the bridge for Zenject dependency injection and lifecycle management. They often serve as the ViewModel or its owner.
- **Reactive Flow**: State changes in the ViewModel automatically notify the View through generated events.

## ViewModel Implementation

### Basic Structure
ViewModels must be `partial` classes decorated with `[ViewModel]`.

```csharp
using Aspid.MVVM;

[ViewModel]
public partial class PlayerStatsViewModel
{
    [Bind] private int _health;
    [Bind] private string _playerName;
}
```

### Binding Attributes
The Source Generator creates properties and change events based on field attributes.

| Attribute | Mode | Description |
|-----------|------|-------------|
| `[Bind]` | Auto | Default binding (usually OneWay or TwoWay depending on target). |
| `[OneWayBind]` | OneWay | ViewModel → View. Property is read-only for the View. |
| `[TwoWayBind]` | TwoWay | Bidirectional sync between ViewModel and View. |
| `[OneTimeBind]` | OneTime | Set once during initialization. |
| `[OneWayToSource]` | ToSource | View → ViewModel (e.g., user input). |

**Naming Convention**: The Source Generator converts `_field`, `m_field`, `s_field`, or `field` into a PascalCase property (e.g., `_health` becomes `Health`).

### Dependent Properties
Use `[BindAlso]` to notify properties that depend on other bound values.

```csharp
[ViewModel]
public partial class ProfileViewModel
{
    [BindAlso(nameof(FullName))]
    [Bind] private string _firstName;

    [BindAlso(nameof(FullName))]
    [Bind] private string _lastName;

    public string FullName => $"{_firstName} {_lastName}";
}
```

## Commands

Use `[RelayCommand]` to generate `ICommand` implementations from methods.

```csharp
[ViewModel]
public partial class LoginViewModel
{
    private bool CanLogin() => !string.IsNullOrEmpty(Username);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private void Login()
    {
        // Login logic
    }
}
```

- **CanExecute**: Can reference a `bool` property or a parameterless method.
- **Parameters**: RelayCommands support up to 4 parameters.

## Collections

Use `ObservableList<T>` and `ObservableDictionary<TKey, TValue>` for dynamic UI elements like lists or grids. These collections notify the View when items are added, removed, or moved.

```csharp
[OneTimeBind] 
private readonly ObservableList<ItemViewModel> _items = new();
```

## Zenject Integration

### Configuration
Ensure `ASPID_MVVM_ZENJECT_INTEGRATION` is defined in Scripting Define Symbols. This enables automatic ViewModel resolution for `Window` and `Presenter` classes.

### Binding ViewModels and Presenters
Presenters and ViewModels should usually be bound as `AsSingle()` or `AsCached()` and marked `NonLazy()` if they drive logic independently.

```csharp
public class UIInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Bind the Presenter
        Container.BindInterfacesAndSelfTo<MainMenuPresenter>().AsSingle().NonLazy();
        
        // Bind the ViewModel if it's a separate class
        Container.BindInterfacesAndSelfTo<MainMenuViewModel>().AsSingle();
    }
}
```

## Project-Specific Notes

- **UISystem Flow**: Always use `UISystem.Start<TPresenter, TWindow>(id)` to ensure correct DI container hierarchy and lifecycle management.
- **Lifecycle Management**: ViewModels or Presenters used in `UIBase` (windows/popups) must correctly implement `Subscribe()` and `Unsubscribe()` to prevent memory leaks in reactive bindings.
- **Async Operations**: Prefer `UniTask` for async logic within ViewModels or Presenters.

## Anti-patterns

- **Direct Unity References**: Never pass `GameObject` or `Transform` into a ViewModel. Use IDs or data structures instead.
- **Missing `partial`**: Forgetting the `partial` keyword on a `[ViewModel]` class breaks source generation.
- **Direct Field Modification**: Modify state through the generated properties (e.g., `Health = 100`) instead of the private fields (`_health = 100`) to ensure notification events are fired.
- **Heavy Logic in Constructor**: Keep ViewModel constructors lightweight. Use `IInitializable` or custom "Init" methods for heavy setup.
- **Reflection-Based Binding**: Avoid using reflection for bindings where Aspid.MVVM's source-generated binders are available, as it defeats the performance benefits of the framework.
