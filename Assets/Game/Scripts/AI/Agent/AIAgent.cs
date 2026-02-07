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
using Units.Config;
using Units.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Agent
{
    public class AIAgent : MonoBehaviour
    {
        [SerializeField] private AIConfig _config;
        [SerializeField] private AgentMover _agentMover;
        
        private AIPlanner _planner;
        private List<ActionBase> _availableActions = new List<ActionBase>();
        private List<GoalBase> _availableGoals = new List<GoalBase>();
        
        private IKnowledge _knowledgeBase;
        private GoalBase _currentGoal;
        private Queue<ActionBase> _actionPlan;
        private ActionBase _currentAction;

        private SensorHolder _sensorHolder;

        public void Constructor(
            string teamKey,
            UnitBaseConfig unitConfig,
            IHealth health
            )
        {
            _sensorHolder = new SensorHolder(
                new HealthSensor(health),
                new VisualSensor(transform, teamKey, _config)
                );
            
            _planner = new AIPlanner();
            _knowledgeBase = new AIKnowledge(_sensorHolder);

            _availableActions = new List<ActionBase>()
            {
                new AttackAction(transform, 1, 1, 10), //TODO from unit or else 
                new MoveToAction(transform, _agentMover)
            };
            
            _availableGoals = new List<GoalBase>()
            {
                new IdleGoal()
            };
        }

        private void OnEnable()
        {
            _knowledgeBase.Changed += KnowledgeUpdated;
        }

        private void OnDisable()
        {
            _knowledgeBase.Changed -= KnowledgeUpdated;
        }

        private void Update()
        {
            if (_currentAction != null)
            {
                bool isComplete = _currentAction.Perform(Time.deltaTime);

                if (isComplete)
                {
                    Debug.Log($"<color=green>Action completed: {nameof(_currentAction)}</color>");
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
                
                if (_currentAction.IsValid()) 
                {
                    Debug.Log($"Starting action: {nameof(_currentAction)}");
                    _currentAction.OnStart();
                    return;
                }
                else
                {
                    Debug.LogWarning("Plan failed: Next action is invalid.");
                    _actionPlan.Clear();
                    _currentAction = null;
                }
            }

            if (_actionPlan == null || _actionPlan.Count == 0)
            {
                CalculateNewGoalAndPlan();
            }
        }

        private void KnowledgeUpdated()
        {
            
        }

        private void CalculateNewGoalAndPlan()
        {
            GoalBase bestGoal = GetBestGoal();
            if (bestGoal == null) 
                return;

            if (_currentGoal != bestGoal || _actionPlan == null)
            {
                _currentGoal = bestGoal;
                _currentGoal.OnGoalActivated();

                Debug.Log($"Planning for goal: {nameof(_currentGoal)} (Priority: {_currentGoal.GetPriority(_knowledgeBase.GetAllFacts())})");

                Queue<ActionBase> plan = 
                    _planner.Plan(this, _availableActions, _knowledgeBase.GetAllFacts(), _currentGoal);

                if (plan != null)
                {
                    _actionPlan = plan;
                    Debug.Log($"<color=cyan>Plan found with {_actionPlan.Count} steps.</color>");
                }
                else
                {
                    Debug.LogWarning($"<color=red>No plan found for goal: {nameof(_currentGoal)}</color>");
                    _currentGoal.OnGoalDeactivated();
                    _currentGoal = null;
                }
            }
        }

        private GoalBase GetBestGoal()
        {
            GoalBase bestGoal = null;
            float highestPriority = -1;
            
            foreach (GoalBase goal in _availableGoals)
            {
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