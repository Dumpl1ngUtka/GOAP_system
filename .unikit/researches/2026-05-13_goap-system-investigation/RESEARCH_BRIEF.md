# RESEARCH_BRIEF: Исправление системы GOAP и поведения AIAgent

## CONTEXT
Project: Unity 6, Zenject, UniTask
Feature: GOAP AIAgent & KillEnemyGoal
Scope: AIAgent + AIPlanner + KillEnemyGoal + AgentMover
Stop condition: Не включает рефакторинг других целей (HealAlly, MineResource) или сенсоров.
Research: .unikit/researches/2026-05-13_goap-system-investigation/RESEARCH_RESULT.md

---
## CONSTRAINTS
- MUST: Использовать хранение цели (target) в поле класса `GoalBase` при расчете приоритета.
- MUST: Реализовать логику реактивности в `AIAgent.Update` для прерывания планов при появлении более приоритетных целей.
- MUST: Добавить в `AgentMover` механизм обнаружения застревания (Stuck Detection) по тайм-ауту или дельте перемещения.
- FORBIDDEN: Запрещено возвращать пустой `GetDesiredState()` в целях, если их приоритет выше 0.
- FORBIDDEN: Запрещено блокировать `Update` агента бесконечным выполнением действия без возможности перепланирования.

---
## INTERFACES
N/A

---
## KEY PATTERNS
N/A

---
## DEPENDENCY GRAPH
[AgentMover]
[GoalBase]
[KillEnemyGoal] <- [GoalBase]
[AIAgent] <- [AgentMover] <- [GoalBase]
[AIPlanner]

---
## FILES

### CREATE
N/A

### MODIFY
| Path | Change |
|------|--------|
| `Assets/Game/AI/Scripts/Agent/AIAgent.cs` | `+ReevaluatePlan()`, модификация `Update` |
| `Assets/Game/AI/Scripts/Planner/AIPlanner.cs` | `+DiscoverTargetsFromKnowledge()` в `Plan` |
| `Assets/Game/AI/Scripts/Goal/Implementation/KillEnemyGoal.cs` | `+IObjectForAI _currentTarget`, фикс `GetDesiredState` |
| `Assets/Game/Scripts/Units/Mover/AgentMover.cs` | `+float _stuckTimer`, `+bool IsStuck` |

---
## DI BINDINGS
N/A

---
## OUT OF SCOPE
- Рефакторинг `HealAllyGoal` и `MineResourceGoal` (вынесено в отдельные задачи).
- Оптимизация производительности `VisualSensor`.
- Реализация слоев `Squad` и `CommanderAgent`.
