# TASKS: Исправление поведения и реактивности системы GOAP

## Overview
Цель этой задачи — сделать поведение юнитов AIAgent автономным и реактивным. Мы исправим критическую ошибку в `KillEnemyGoal`, которая заставляла ИИ игнорировать врагов, добавим механизм прерывания текущих действий при появлении более важных целей и внедрим детекцию "застревания" юнита при движении.

## Based on
### 2026-05-13_goap-system-investigation
- **Attached**: 2026-05-13 14:00
- `RESEARCH_BRIEF.md` — structured technical context
- `RESEARCH_RESULT.md` — full research (consult when brief is unclear)
- `RESEARCH_SOURCE.md` — original exploration dialogue

`PLAN-BRIEF.md` (in this folder)

## Settings
- Testing: yes
- Docs: no

## Roadmap Linkage
Milestone: none
Rationale: Skipped by user

## Checklist

### Phase 1: Core Goal Infrastructure (Foundation)
- [ ] Task 1.1 — Modify `GoalBase.cs` to add `_currentTarget` field
  WHY: Ensures that a goal can "remember" the target it found during priority calculation for the planning phase.
  Files: `Assets/Game/AI/Scripts/Goal/GoalBase.cs`
- [ ] Task 1.2 — Fix `KillEnemyGoal.cs` logic and target persistence
  WHY: Fixes the bug where autonomous agents ignored enemies because `GetDesiredState` returned empty conditions.
  Files: `Assets/Game/AI/Scripts/Goal/Implementation/KillEnemyGoal.cs`
- [ ] Task 1.3 — Expand target discovery in `AIPlanner.cs`
  WHY: Allows the planner to discover all objects in the world (from KnowledgeBase) as valid targets for actions, enabling complex multi-step plans.
  Files: `Assets/Game/AI/Scripts/Planner/AIPlanner.cs`

### Phase 2: Movement & Stuck Detection
- [ ] Task 2.1 — Add stuck detection parameters to `AIConfig.cs`
  WHY: Provides configurable thresholds (distance, interval) for the stuck detection logic.
  Files: `Assets/Game/AI/Scripts/Configs/AIConfig.cs`
- [ ] Task 2.2 — Implement Stuck Detection in `AgentMover.cs`
  WHY: Prevents agents from being locked in a movement action forever if they hit an obstacle.
  Files: `Assets/Game/Scripts/Units/Mover/AgentMover.cs`
- [ ] Task 2.3 — Fix `MoveToAction.cs` completion logic
  WHY: Current implementation returns `IsMoving` (true), causing the action to finish instantly. It must wait until movement stops.
  Files: `Assets/Game/AI/Scripts/Actions/Implementation/MoveToAction.cs`

### Phase 3: Agent Reactivity (The Brain)
- [ ] Task 3.1 — Implement `ReevaluatePlan` in `AIAgent.cs`
  WHY: Adds the ability to abort low-priority actions (like patrolling) immediately when a threat (enemy) is detected.
  Files: `Assets/Game/AI/Scripts/Agent/AIAgent.cs`
- [ ] Task 3.2 — Connect `OnStuck` event to AIAgent re-planning
  WHY: Ensures the agent immediately tries a different plan if the current movement is blocked.
  Files: `Assets/Game/AI/Scripts/Agent/AIAgent.cs`

### Phase 4: Verification (Tests)
- [ ] Task 4.1 — Create Assembly Definitions for AI and Tests
  WHY: Required by Unity to run NUnit tests and properly separate production code from test code.
  Files: `Assets/Game/AI/Scripts/AI.asmdef`, `Assets/Game/AI/Tests/EditMode/AI.Tests.asmdef`
- [ ] Task 4.2 — Implement unit tests for `KillEnemyGoal`
  WHY: Empirically verifies that the fix for Task 1.2 works and prevents regressions in goal logic.
  Files: `Assets/Game/AI/Tests/EditMode/KillEnemyGoalTests.cs`

## Commit Plan
- Checkpoint 1: Tasks 1.1 - 1.3 (Goal Fixes & Planner Expansion)
- Checkpoint 2: Tasks 2.1 - 2.3 (Movement Fixes & Stuck Detection)
- Checkpoint 3: Tasks 3.1 - 3.2 (Reactivity & Agent Integration)
- Checkpoint 4: Tasks 4.1 - 4.2 (Assemblies & Final Tests)

## Dependency Graph
[Phase 1] --> [Phase 3]
[Phase 2] --> [Phase 3]
[Phase 1, 2, 3] --> [Phase 4]

## Total Estimated Effort
8 tasks (~2-3 hours)
