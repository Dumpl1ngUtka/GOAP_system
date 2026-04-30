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
using Units;
using Units.Mover;
using Units.UnitClasses;
using UnityEngine;

namespace AI.Agent
{
    public class AIAgent : MonoBehaviour
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

        private SensorHolder _sensorHolder;

        public void Constructor(
            string teamKey,
            Unit unit)
        {
            _sensorHolder = new SensorHolder(
                new HealthSensor(unit.Health),
                new VisualSensor(transform, teamKey, _config),
                new InventorySensor(unit.Inventory)
                );

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
                _knowledgeBase.Changed += KnowledgeUpdated;
                _sensorHolder.Start();
            }
        }
        
        public void AssignOrder(GoalBase newOrder)
        {
            if (_assignedOrder == newOrder) return;

            _assignedOrder = newOrder;
            Debug.Log($"<color=yellow>Agent {gameObject.name} received new order: {newOrder}</color>");
            
            // Сбрасываем текущий план, чтобы юнит немедленно переосмыслил свои цели
            if (_currentAction != null)
            {
                _currentAction.OnStop();
                _currentAction = null;
            }
            _actionPlan?.Clear();
            _currentGoal?.OnGoalDeactivated();
            _currentGoal = null;
            
            CalculateNewGoalAndPlan();
        }

        public void ClearOrder()
        {
            _assignedOrder = null;
        }


        private void OnEnable()
        {
            if (_knowledgeBase != null)
                _knowledgeBase.Changed += KnowledgeUpdated;
            
            if (_sensorHolder != null)
                _sensorHolder.Start();
        }

        private void OnDisable()
        {
            if (_knowledgeBase != null)
                _knowledgeBase.Changed -= KnowledgeUpdated;
            
            if (_sensorHolder != null)
                _sensorHolder.Stop();
        }

        private void Update()
        {
            if (_sensorHolder != null)
                _sensorHolder.Update(Time.deltaTime);

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
            
        }

        private void CalculateNewGoalAndPlan()
        {
            List<GoalBase> noValidGoals = new();
            for (int i = 0; i < _availableGoals.Count; i++)
            {
                GoalBase bestGoal = GetBestGoal(_availableGoals, noValidGoals);
                if (bestGoal == null) 
                    return;

                if (_currentGoal != bestGoal || _actionPlan == null)
                {
                    _currentGoal = bestGoal;
                    _currentGoal.OnGoalActivated();

                    Debug.Log($"Planning for goal: {_currentGoal} (Priority: {_currentGoal.GetPriority(_knowledgeBase.GetAllFacts())})");

                    Queue<ActionBase> plan = 
                        _planner.Plan(_availableStrategies, _knowledgeBase.GetAllFacts(), _currentGoal);

                    if (plan != null)
                    {
                        _actionPlan = plan;
                        Debug.Log($"<color=cyan>Plan found with {_actionPlan.Count} steps.</color>");
                        return;
                    }

                    Debug.LogWarning($"<color=red>No plan found for goal: {_currentGoal}</color>");
                    _currentGoal.OnGoalDeactivated();
                    _currentGoal = null;
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
    }
}