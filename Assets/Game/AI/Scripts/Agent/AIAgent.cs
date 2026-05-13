using System;
using System.Collections.Generic;
using System.Linq;
using AI.Actions;
using AI.Actions.Implementation;
using AI.Configs;
using AI.Goal;
using AI.Goal.Implementation;
using AI.Knowledge;
using AI.Planner;
using AI.Sensors;
using OwnSystems.DamageSystem;
using Units;
using Units.Mover;
using Units.UnitClasses;
using UnityEngine;

namespace AI.Agent
{
    public class AIAgent : MonoBehaviour, IDamageable, IWorldObjectForAI
    {
        public UnitRole Role => _unitRole;
        [SerializeField] private AIConfig _config;
        [SerializeField] private AgentMover _agentMover;
        
        private AIPlanner _planner;
        private List<ActionStrategy> _availableStrategies = new List<ActionStrategy>();
        private List<GoalBase> _availableGoals = new List<GoalBase>();
        
        private IKnowledge _knowledgeBase;
        private GoalBase _currentGoal;
        private Queue<ActionBase> _actionPlan;
        private ActionBase _currentAction;
        private GoalBase _assignedOrder; 
        private UnitRole _unitRole;
        private Unit _unit;
        private string _teamKey;

        private SensorHolder _sensorHolder;
        private bool _needsReevaluation;

        public void Constructor(
            string teamKey,
            Unit unit)
        {
            _agentMover.OnStuck += HandleStuck;

            _sensorHolder = new SensorHolder(
                new HealthSensor(unit.Health),
                new VisualSensor(transform, teamKey, _config),
                new InventorySensor(unit.Inventory)
                );

            _unit = unit;
            _teamKey = teamKey;
            _unitRole = unit.Config.Role;
            _planner = new AIPlanner();
            _knowledgeBase = new AIKnowledge(_sensorHolder);

            _availableStrategies = new List<ActionStrategy>()
            {
                new IdleActionStrategy(), 
                new GatherResourceActionStrategy(transform, 0.1f), //TODO from unit or else 
                new AttackActionStrategy(transform, 1.0f, 10), //TODO from unit or else 
                new MoveToActionStrategy(_agentMover),
                new PatrolActionStrategy(_agentMover, transform, 10f) 
            };
            
            _availableGoals = new List<GoalBase>()
            {
                new IdleGoal(),
                new SurviveGoal(),
                new PickUpItemGoal(),
                new HealAllyGoal(),
                new KillEnemyGoal(),
                new PatrolGoal() 
            };

            if (isActiveAndEnabled)
            {
                Debug.Log("IS ACTIVE AND ENABLED");
                _knowledgeBase.Changed += KnowledgeUpdated;
                _knowledgeBase.Start();
            }
        }
        
        private void HandleStuck()
        {
            Debug.LogWarning($"<color=red>Agent {gameObject.name} is STUCK. Clearing plan.</color>");
            ClearCurrentPlan();
            CalculateNewGoalAndPlan();
        }

        private void ClearCurrentPlan()
        {
            if (_currentAction != null)
            {
                _currentAction.OnStop();
                _currentAction = null;
            }
            _actionPlan?.Clear();
            _currentGoal?.OnGoalDeactivated();
            _currentGoal = null;
        }

        public void AssignOrder(GoalBase newOrder)
        {
            if (_assignedOrder == newOrder) return;

            _assignedOrder = newOrder;
            Debug.Log($"<color=yellow>Agent {gameObject.name} received new order: {newOrder}</color>");
            
            // Сбрасываем текущий план, чтобы юнит немедленно переосмыслил свои цели
            ClearCurrentPlan();
            
            CalculateNewGoalAndPlan();
        }

        public void ClearOrder()
        {
            _assignedOrder = null;
        }


        private void OnEnable()
        {
            if (_knowledgeBase != null)
            {
                _knowledgeBase.Changed += KnowledgeUpdated;
                _knowledgeBase.Start();
            }
        }

        private void OnDisable()
        {
            if (_knowledgeBase != null)
            {
                _knowledgeBase.Changed -= KnowledgeUpdated;
                _knowledgeBase.Stop();
            }
        }

        private void Update()
        {
            if (_sensorHolder != null)
                _sensorHolder.Update(Time.deltaTime);

            if (_needsReevaluation)
            {
                _needsReevaluation = false;
                ReevaluatePlan();
            }

            if (_currentAction != null)
            {
                bool isComplete = _currentAction.Perform(Time.deltaTime);

                if (isComplete)
                {
                    Debug.Log($"<color=green>Action completed: {_currentAction}</color>");
                    _currentAction.OnStop();
                    
                    _currentAction = null;
                }
                else
                {
                    return;
                }
            }

            if (_actionPlan != null && _actionPlan.Count > 0)
            {
                _currentAction = _actionPlan.Dequeue();
                
                // IsValid check removed as it was not in ActionBase, or needs to be added back if needed.
                // Assuming OnStart is always safe to call if plan was valid.
                Debug.Log($"Starting action: {_currentAction}");
                _currentAction.OnStart();
            }
            else if (_actionPlan == null || _actionPlan.Count == 0)
            {
                CalculateNewGoalAndPlan();
            }
        }

        private void KnowledgeUpdated()
        {
            _needsReevaluation = true;
        }

        private void ReevaluatePlan()
        {
            List<GoalBase> goalsToEvaluate = new List<GoalBase>(_availableGoals);
            if (_assignedOrder != null && !goalsToEvaluate.Contains(_assignedOrder))
            {
                goalsToEvaluate.Add(_assignedOrder);
            }

            GoalBase bestGoal = GetBestGoal(goalsToEvaluate, new List<GoalBase>());
            if (bestGoal == null) return;

            // Если текущая цель изменилась ИЛИ у новой цели приоритет значительно выше
            if (_currentGoal != bestGoal)
            {
                float currentPriority = _currentGoal != null ? _currentGoal.GetPriority(_knowledgeBase.GetAllFacts()) : 0;
                float newPriority = bestGoal.GetPriority(_knowledgeBase.GetAllFacts());

                if (newPriority > currentPriority)
                {
                    Debug.Log($"<color=orange>Re-evaluating plan: {(_currentGoal?.GetType().Name ?? "None")} -> {bestGoal.GetType().Name}</color>");
                    ClearCurrentPlan();
                    CalculateNewGoalAndPlan();
                }
            }
        }

        private void CalculateNewGoalAndPlan()
        {
            List<GoalBase> noValidGoals = new();
    
            // 1. Создаем ОБЩИЙ список целей: базовые цели юнита + приказ от Командира
            List<GoalBase> goalsToEvaluate = new List<GoalBase>(_availableGoals);
    
            if (_assignedOrder != null && !goalsToEvaluate.Contains(_assignedOrder))
            {
                goalsToEvaluate.Add(_assignedOrder); // Добавляем приказ в мозговой процесс юнита
            }

            // 2. Ограничиваем цикл количеством всех доступных целей
            int maxAttempts = goalsToEvaluate.Count;
            for (int i = 0; i < maxAttempts; i++)
            {
                // ВАЖНО: передаем именно goalsToEvaluate, а не старый _availableGoals!
                GoalBase bestGoal = GetBestGoal(goalsToEvaluate, noValidGoals); 
        
                if (bestGoal == null) 
                    return; // Больше нет целей для проверки

                if (_currentGoal != bestGoal || _actionPlan == null)
                {
                    _currentGoal = bestGoal;
                    _currentGoal.OnGoalActivated();

                    // Пишем в консоль, какую цель мы сейчас пытаемся спланировать
                    Debug.Log($"Planning for goal: {_currentGoal.GetType().Name} (Priority: {_currentGoal.GetPriority(_knowledgeBase.GetAllFacts())})");

                    Queue<ActionBase> plan = 
                        _planner.Plan(_availableStrategies, _knowledgeBase.GetAllFacts(), _currentGoal);

                    if (plan != null)
                    {
                        _actionPlan = plan;
                        Debug.Log($"<color=cyan>Plan found for {_currentGoal.GetType().Name} with {_actionPlan.Count} steps.</color>");
                        return;
                    }

                    Debug.LogWarning($"<color=red>No plan found for goal: {_currentGoal.GetType().Name}</color>");
                    _currentGoal.OnGoalDeactivated();
                    _currentGoal = null;
            
                    // Заносим цель в черный список, чтобы на следующей итерации цикла выбрать цель с приоритетом пониже
                    noValidGoals.Add(bestGoal);
                }
            }
        }
        
        private GoalBase GetBestGoal(List<GoalBase> availableGoals, List<GoalBase> noValidGoals)
        {
            GoalBase bestGoal = null;
            float highestPriority = -1;
            
            foreach (GoalBase goal in availableGoals)
            {
                if (noValidGoals.Contains(goal))
                    continue;
                
                float priority = goal.GetPriority(_knowledgeBase.GetAllFacts());
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    bestGoal = goal;
                }
            }
            
            return bestGoal;
        }

        public void ApplyDamage(float damage)
        {
            _unit.Health.ApplyDamage((ushort)damage);
        }

        public IEnumerable<string> GetTags()
        {
            List<string> keys = new List<string>();
            keys.Add(_teamKey);
            return keys;
        }

        public Transform GetWorldTransform()
        {
            return transform;
        }
    }
}