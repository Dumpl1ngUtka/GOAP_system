# PLAN-BRIEF: Исправление поведения и реактивности системы GOAP

## CONTEXT
Project: Unity 6, Zenject, UniTask
Feature: GOAP Behavior & Reactivity Fix
Scope: AIAgent + AIPlanner + GoalBase + AgentMover + KillEnemyGoal
Stop condition: Не включает реализацию новых действий или сенсоров.
Research: .unikit/researches/2026-05-13_goap-system-investigation/RESEARCH_RESULT.md

---
## CONSTRAINTS
- MUST: Использовать `_currentTarget` в `GoalBase` для хранения объекта, выбранного в `GetPriority`.
- MUST: Реализовать прерываемость действий в `AIAgent.Update` через сравнение приоритетов.
- MUST: Внедрить событие `OnStuck` в `AgentMover` для сигнализации о невозможности завершить движение.
- MUST: В `AIPlanner.Plan` собирать всех потенциальных кандидатов (targets) из фактов знаний, а не только из желаемого состояния цели.
- FORBIDDEN: Запрещено изменять логику А* в `AIPlanner` (только расширение списка кандидатов).
- FORBIDDEN: Запрещено использовать `async void` в логике ИИ (только `UniTask` или `UniTaskVoid`).

---
## INTERFACES

### IAgentMover [MODIFY]
```csharp
// Units.Mover
public interface IAgentMover
{
    // ADD:
    event Action OnStuck; // Срабатывает, когда юнит не может достичь цели
}
```

### GoalBase [MODIFY]
```csharp
// AI.Goal
public abstract class GoalBase
{
    // ADD:
    protected IObjectForAI _currentTarget; // Кэшированный объект для планирования
    
    public virtual void OnGoalDeactivated()
    {
        _currentTarget = null; // Очистка кэша
    }
}
```

---
## KEY PATTERNS

### Stuck Detection (AgentMover)
```csharp
// HandleMovement()
_stuckTimer += Time.fixedDeltaTime;
if (_stuckTimer >= _config.StuckCheckInterval) {
    float distSqr = (transform.position - _lastCheckedPos).sqrMagnitude;
    if (distSqr < _config.StuckDistanceThresholdSqr) {
        OnStuck?.Invoke();
    }
    _lastCheckedPos = transform.position;
    _stuckTimer = 0;
}
```

### Reactive Re-planning (AIAgent)
```csharp
// Update()
if (_needsReevaluation) {
    ReevaluatePlan();
    _needsReevaluation = false;
}
```

---
## DEPENDENCY GRAPH
[AgentMover]
  <- [AIConfig] ( thresholds)
[GoalBase]
[KillEnemyGoal] <- [GoalBase]
[AIPlanner]
[AIAgent] 
  <- [AIPlanner]
  <- [AgentMover]
  <- [GoalBase]

---
## FILES

### CREATE
| Path | Type | Notes |
|------|------|-------|
| `Assets/Game/AI/Scripts/AI.asmdef` | asmdef | Production assembly |
| `Assets/Game/AI/Tests/EditMode/AI.Tests.asmdef` | asmdef | Test assembly |
| `Assets/Game/AI/Tests/EditMode/KillEnemyGoalTests.cs` | class | NUnit tests |

### MODIFY
| Path | Change |
|------|--------|
| `Assets/Game/AI/Scripts/Agent/AIAgent.cs` | Добавление `ReevaluatePlan`, фикс `Update` |
| `Assets/Game/AI/Scripts/Planner/AIPlanner.cs` | Расширение `potentialTargets` в методе `Plan` |
| `Assets/Game/AI/Scripts/Goal/GoalBase.cs` | Добавление поля `_currentTarget` |
| `Assets/Game/AI/Scripts/Goal/Implementation/KillEnemyGoal.cs` | Использование `_currentTarget` в `GetDesiredState` |
| `Assets/Game/AI/Scripts/Actions/Implementation/MoveToAction.cs` | Фикс логики `Perform` (`!IsMoving`) |
| `Assets/Game/Scripts/Units/Mover/AgentMover.cs` | Реализация Stuck Detection и `OnStuck` |
| `Assets/Game/AI/Scripts/Configs/AIConfig.cs` | Добавление параметров Stuck Detection |

---
## DI BINDINGS
N/A (Core GOAP components are instantiated manually in AIAgent.Constructor)

---
## OUT OF SCOPE
- Рефакторинг `HealAllyGoal`, `MineResourceGoal`, `PickUpItemGoal`.
- Оптимизация сенсоров.
- Реализация логики Squad/Commander.
