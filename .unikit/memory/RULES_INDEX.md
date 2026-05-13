# Rules Index

Knowledge base rules for the project. Located in `.unikit/memory/`.

## How to use this index

You were directed here by a skill or subagent. The name after "instructions for" in that directive is **your identity** — use it when checking the Required By column below.

### Override Priority (highest wins)

1. **`.unikit/RULES.md`** — project-specific overrides (always wins)
2. **`.unikit/ARCHITECTURE.md`** — project architecture decisions
3. **Core rules** (`.unikit/memory/core/`) — universal best practices
4. **Stack rules** (`.unikit/memory/stack/`) — framework-specific knowledge

When a project rule in RULES.md or ARCHITECTURE.md conflicts with a core or stack rule, the project rule wins.

### Step 1: Load RULES.md
Read `.unikit/RULES.md` before loading any rule below. It contains project-specific overrides that take highest priority.

### Step 2: Load Core rules
For each row in the Core table, check the **Required By** column:
- `all` → **MUST load** (mandatory for every skill and subagent)
- Contains your name → **MUST load**
- Does NOT contain your name and is NOT `all` → **skip**

### Step 3: Load Stack rules (on demand)
Load ONLY when the current task involves the framework described in the **Load When** column.

## Core (`.unikit/memory/core/`)

| File | Description | Required By | Load When |
|------|-------------|-------------|-----------|
| code-style.md | Universal C#/Unity code style conventions — naming, access modifiers, member ordering, class structure, formatting, component approach, documentation. | all | writing or reviewing any C# code, creating new classes, checking code style. |
| design-principles.md | Universal software design principles — SOLID, GRASP, KISS, DRY, inheritance guidelines, SRP decision framework, method design, defensive programming. | all | designing systems, creating new classes, choosing architecture patterns, deciding when to split or merge classes, method design, defensive programming. |
| folders-structure.md | Project folder organization, module structure, namespace conventions, external asset boundaries | all | Creating new scripts, choosing file location, setting namespace, creating folders, module structure, folder layout |
| performance.md | Rules for performance optimization — memory/GC, caching, ZLinq, strings, object pooling, delegates, math, physics, UI optimization, mobile specifics. | all | performance issues, optimization, hot paths, memory/GC, pooling, ZLinq. |
| testing.md | Rules for NUnit unit tests — AAA pattern, test class structure, naming, test doubles (Fake/Stub/Mock), parameterized tests, boundary conditions, assembly definitions, ScriptableObject in tests, PlayMode tests. | all | writing or reviewing unit tests, creating test doubles, setting up test assemblies. |

## Stack (`.unikit/memory/stack/`)

| File | Description | Load When |
|------|-------------|-----------|
| aspid-mvvm.md | UI development using Aspid.MVVM framework — source-generated ViewModels, data binding, commands, observable collections, and Zenject integration. | designing UI windows, creating ViewModels, binding data to uGUI elements, implementing UI logic, wiring MVVM components with Zenject. |
| dotween.md | DOTween Pro tweening library for Unity — tween creation and chaining, Sequence composition, tween lifecycle and control, global configuration, safe mode, recycling, callbacks, ease types, and DOTween Pro visual components (DOTweenAnimation, DOTweenPath, DOTweenVisualManager). | animating values with DOTween, creating tweens or sequences, chaining SetEase/SetLoops/SetDelay, wiring tween callbacks, managing tween lifecycle (kill/pause/complete), configuring DOTween initialization, using DOTweenAnimation or DOTweenPath components, debugging tween errors or memory leaks, animating UI with DOTween. |
| input-system.md | Unity Input System package (com.unity.inputsystem) — action setup and lifecycle, callback patterns, InputActionAsset and generated wrappers, PlayerInput component, control schemes, runtime rebinding, and update mode configuration. | handling player input with the new Input System, creating InputActions or InputActionAssets, wiring input callbacks, setting up control schemes or device switching, implementing runtime rebinding, debugging input not firing, choosing between PlayerInput and manual action management. |
| unitask.md | UniTask async/await rules — CancellationToken, exception handling, timeouts, UniTaskVoid, API conventions. | writing async/await code with UniTask — propagating CancellationToken through async chains, handling OperationCanceledException, applying timeouts, choosing between UniTask and UniTaskVoid for fire-and-forget |
| unity-ai-navigation.md | Navigation and pathfinding using the com.unity.ai.navigation package (AI Navigation 1.x/2.x). Covers NavMeshSurface, NavMeshAgent, NavMeshLink, and runtime baking patterns. | implementing character movement, building NavMeshes, configuring agents, or optimizing pathfinding for Unity 2022.2+ or Unity 6 projects. |
| unity-mcp.md | MCP for Unity bridge by CoplayDev — exposing custom C# tools to AI agents, scene and asset manipulation via MCP, bridge configuration, and tool authoring patterns. | creating custom MCP tools for AI, exposing project-specific methods to Claude/Cursor/Copilot, configuring the Unity-MCP bridge, or debugging tool discovery and execution. |
| urp.md | Universal Render Pipeline (URP) configuration, performance optimization, custom Scriptable Renderer Features, and shader development conventions. | configuring graphics settings, optimizing rendering performance, authoring custom render passes or shaders, setting up post-processing volumes, or debugging rendering issues in URP. |
| zenject.md | Zenject dependency injection framework for Unity — binding API, container hierarchy, injection methods, installer types, entry points, execution order, and composition root patterns. | wiring dependencies with Zenject, creating installers or bindings, choosing lifetimes, injecting into MonoBehaviours or plain C# classes, implementing IInitializable/ITickable/IDisposable, debugging container resolution errors, managing scene or project-level container scope. |
