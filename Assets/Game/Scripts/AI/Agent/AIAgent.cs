using System;
using System.Collections.Generic;
using System.Linq;
using AI.Actions;
using AI.Actions.Implementation;
using AI.Goal;
using AI.Knowledge;
using AI.Planner;
using AI.Sensors;
using UnityEngine;

namespace AI.Agent
{
    public class AIAgent : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private string _currentGoalName;
        [SerializeField] private string _currentActionName;

        private AIPlanner _planner;
        private List<ActionBase> _availableActions = new List<ActionBase>();
        private List<GoalBase> _availableGoals = new List<GoalBase>();
        
        private IKnowledge _knowledgeBase;
        private GoalBase _currentGoal;
        private Queue<ActionBase> _actionPlan;
        private ActionBase _currentAction;

        private SensorHolder _sensorHolder;

        private void Constructor(
            Unit.Unit unit
            )
        {
            _sensorHolder = new SensorHolder(
                new HealthSensor(unit.Health)
                );
            
            _planner = new AIPlanner();
            _knowledgeBase = new AIKnowledge(_sensorHolder);

            _availableActions = new List<ActionBase>()
            {
                new AttackAction(),
                new MoveToAction()
            };
            _availableGoals = GetComponents<GoalBase>().ToList();
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
                    Debug.Log($"<color=green>Action completed: {_currentAction.ActionName}</color>");
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
                _currentActionName = _currentAction.ActionName;
                
                if (_currentAction.IsValid()) 
                {
                    Debug.Log($"Starting action: {_currentAction.ActionName}");
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
            GoalBase bestGoal = null;
            float highestPriority = -1;

            foreach (var goal in _availableGoals)
            {
                if (goal.ValidateAndCalculatePriority(_knowledgeBase.GetAllFacts()))
                {
                    if (goal.Priority > highestPriority)
                    {
                        highestPriority = goal.Priority;
                        bestGoal = goal;
                    }
                }
            }

            if (bestGoal == null || highestPriority <= 0)
            {
                _currentGoalName = "Idle";
                _currentActionName = "None";
                return;
            }

            if (_currentGoal != bestGoal || _actionPlan == null)
            {
                _currentGoal = bestGoal;
                _currentGoalName = _currentGoal.Name;
                _currentGoal.OnGoalActivated();

                Debug.Log($"Planning for goal: {_currentGoal.Name} (Priority: {_currentGoal.Priority})");

                Queue<ActionBase> plan = 
                    _planner.Plan(this, _availableActions, _knowledgeBase.GetAllFacts(), _currentGoal);

                if (plan != null)
                {
                    _actionPlan = plan;
                    Debug.Log($"<color=cyan>Plan found with {_actionPlan.Count} steps.</color>");
                }
                else
                {
                    Debug.LogWarning($"<color=red>No plan found for goal: {_currentGoal.Name}</color>");
                    _currentGoal.OnGoalDeactivated();
                    _currentGoal = null;
                }
            }
        }
    }
}