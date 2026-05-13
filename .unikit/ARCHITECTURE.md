# Architecture: Feature-Based with Modular UI

## Project

Goap_aa is a multiplayer strategy game where players deploy autonomous units managed by a multi-level GOAP (Goal-Oriented Action Planning) AI system. The goal is to destroy the opponent's main structure while defending your own. Target platform: Android.

## Tech Stack

| Category | Technology |
|----------|-----------|
| Engine | Unity 6000.0.58f2 / C# / URP |
| DI | Zenject (Extenject) |
| Async | UniTask |
| UI | Aspid.MVVM (Custom Modular UI) |
| Animation | DOTween |
| Navigation | AI Navigation |
| Input | Unity Input System |

## Documentation Sources

When you need up-to-date API docs for these libraries:

- **Zenject**: Use context7 to query "Zenject DI Unity documentation and binding examples".
- **UniTask**: Use context7 to query "UniTask async/await Unity documentation".
- **DOTween**: Use context7 to query "DOTween Pro Unity documentation and sequences".
- **AI Navigation**: Use context7 to query "Unity AI Navigation package documentation".

## Architecture Overview

The project follows a **Feature-Based** organization for the core game logic, combined with a **Modular** approach for reusable systems like the UI. 

The architecture is **GOAP-centric**, meaning the AI behavior is the core driver of the gameplay. Logic is separated into independent layers (Agent, Squad, Commander) to manage complexity. 

Dependency Injection (via Zenject) is used to decouple systems and provide a clean way to manage service lifetimes and cross-system communication.

## Folder Structure

```
[Project root]/
├── Assets/Game/Scripts/    [Main game logic organized by system/feature]
│   ├── GOAP/               [Core AI logic: Actions, Goals, Sensors, Planner]
│   ├── Units/              [Unit-specific logic: Health, Mover, Inventory]
│   ├── Services/           [Global systems: Audio, Spawner, SaveLoad, GameStates]
│   ├── Config/             [ScriptableObject-based configurations]
│   ├── Controllers/        [Input handling and camera control]
│   └── Installers/         [Zenject dependency wiring]
├── Assets/Modules/         [Reusable isolated modules with .asmdef boundaries]
│   ├── UI/                 [High-level UI system and presenters]
│   └── UIBase/             [Base UI components and core abstractions]
├── Assets/Plugins/         [Third-party frameworks like DOTween, Zenject]
└── Assets/Settings/        [Render pipeline and project-wide settings]
```

## Dependency Rules

- ✅ `Game` (Assembly-CSharp) can depend on `Modules/UI` and `Modules/UIBase`.
- ✅ `Modules/UI` depends on `Modules/UIBase`.
- ✅ All systems can depend on `Plugins` (Zenject, UniTask, etc.).
- ❌ `Modules` should NOT depend on `Game` (Assembly-CSharp) to ensure reusability.
- ❌ Core logic in `GOAP/` should remain independent of specific `View` implementations.

## Module Boundary Strategy

The project uses **Assembly Definitions (.asmdef)** to enforce boundaries for reusable modules:
- `Assets/Modules/UI/Runtime/UISystem.asmdef`
- `Assets/Modules/UIBase/Runtime/UIBase.asmdef`

These modules are designed to be project-agnostic and are integrated into the main game via Zenject installers (e.g., `UISystemInstaller`).

## Cross-Module Communication

- **Zenject Interfaces**: Primary method for system-to-system communication. Services are bound to interfaces in `ProjectInstaller`.
- **State Machine**: The `GameStateMachine` orchestrates high-level transitions (Bootstrap -> MainMenu -> Game).
- **GOAP Sensors/Facts**: Units perceive the environment via `GoapSensor` and store findings in `GoapKnowledgeBase` as `Fact` objects.
- **Presenters**: Mediate between the game logic and the UI system (found in `Services/GameCard`).

## Key Principles

1. **GOAP-centric AI**: All autonomous behavior must be implemented through the GOAP framework (Goals, Actions, Sensors).
2. **Service-Oriented Logic**: Shared functionality (Audio, Spawning, Saving) must be encapsulated in injectable services, not static singletons.
3. **UniTask for Async**: All non-blocking operations (animations, loading, delays) must use UniTask with proper `CancellationToken` propagation.
4. **Data-Driven Configuration**: Use `ScriptableObject` (in `Assets/Game/Scripts/Config/`) to define game balance and settings.

## Anti-Patterns

- ❌ **Static Singletons**: Use Zenject's `AsSingle()` bindings instead.
- ❌ **Direct View-to-Logic coupling**: Use Presenters or Interfaces to mediate between UI and Game state.
- ❌ **async void**: Always use `async UniTask` or `async UniTaskVoid`.
- ❌ **Manual MonoBehaviour management**: Prefer Zenject's `IInitializable`, `ITickable`, and `IDisposable` for logic lifetime.

## Detailed Rules

For framework-specific rules, coding conventions, and implementation details see:

- **`.unikit/memory/RULES_INDEX.md`** — Index of all framework-specific rule files.
- **`.unikit/memory/core/code-style.md`** — Naming and C# formatting conventions.
- **`.unikit/memory/stack/zenject.md`** — Detailed DI binding and injection rules.
