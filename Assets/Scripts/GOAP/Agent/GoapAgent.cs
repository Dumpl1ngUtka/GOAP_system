using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using UnityEngine;

namespace GOAP.Agent
{
    public class GoapAgent : MonoBehaviour, IGoapAgent
    {
        private const float ReplanInterval = 0.5f;
        
        private IGoapKnowledge _knowledge;
        private IGoapPlanner _planner;
        private IGoapSensor _sensor;
    
        private List<IGoapGoal> _goals = new List<IGoapGoal>();
        private List<IGoapAction> _availableActions = new List<IGoapAction>();
    
        private Queue<IGoapAction> _currentPlan;
        private IGoapAction _currentAction;
        private IGoapGoal _currentGoal;
    
        private float _replanTimer;

        public void Initialize(IGoapKnowledge knowledge, IGoapPlanner planner, IGoapSensor sensor)
        {
            _knowledge = knowledge;
            _planner = planner;
            _sensor = sensor;
        }

        private void Update()
        {
            if (_knowledge == null || _planner == null) return;

            _sensor.UpdateKnowledge();
        
            _replanTimer += Time.deltaTime;
            if (_replanTimer >= ReplanInterval)
            {
                _replanTimer = 0f;
                EvaluateGoals();
            }

            ExecuteCurrentPlan();
        }

        private void EvaluateGoals()
        {
            IGoapGoal bestGoal = null;
            float highestPriority = float.MinValue;

            foreach (var goal in _goals)
            {
                if (goal.IsValid(_knowledge) && goal.Priority > highestPriority)
                {
                    highestPriority = goal.Priority;
                    bestGoal = goal;
                }
            }

            if (bestGoal != _currentGoal)
            {
                SwitchGoal(bestGoal);
            }
        }

        private void SwitchGoal(IGoapGoal newGoal)
        {
            _currentGoal?.OnGoalDeactivated();
            _currentGoal = newGoal;
            _currentGoal?.OnGoalActivated();
            Replan();
        }

        private void ExecuteCurrentPlan()
        {
            if (_currentPlan == null || _currentAction == null) return;

            if (_currentAction.IsDone)
            {
                HandleActionCompletion();
                return;
            }

            if (!_currentAction.CheckProceduralPrecondition(_knowledge))
            {
                AbortCurrentPlan();
                return;
            }

            _currentAction.Perform();
        }

        private void HandleActionCompletion()
        {
            _currentAction.OnExit();
            _currentAction = null;

            if (_currentPlan.Count == 0)
            {
                _currentPlan = null;
                return;
            }

            _currentAction = _currentPlan.Dequeue();
            _currentAction.OnEnter();
        }

        public void AddGoal(IGoapGoal goal)
        {
            if (!_goals.Contains(goal))
            {
                _goals.Add(goal);
            }
        }

        public void RemoveGoal(string goalName)
        {
            _goals.RemoveAll(g => g.Name == goalName);
        }

        public void AddAction(IGoapAction action)
        {
            if (!_availableActions.Contains(action))
            {
                _availableActions.Add(action);
            }
        }

        public void Replan()
        {
            AbortCurrentPlan();

            if (_currentGoal == null || !_currentGoal.IsValid(_knowledge))
            {
                return;
            }

            _currentPlan = _planner.Plan(_knowledge, _currentGoal, _availableActions);

            if (_currentPlan != null && _currentPlan.Count > 0)
            {
                _currentAction = _currentPlan.Dequeue();
                _currentAction.OnEnter();
            }
        }

        public void AbortCurrentPlan()
        {
            if (_currentAction != null)
            {
                _currentAction.OnExit();
                _currentAction = null;
            }

            _currentPlan = null;
        }

        private void OnDestroy()
        {
            AbortCurrentPlan();
            _currentGoal?.OnGoalDeactivated();
        }
    }
}