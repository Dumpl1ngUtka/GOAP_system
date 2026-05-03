using System.Collections.Generic;
using System.Linq;
using AI.Actions;
using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Planner
{
    public class AIPlanner
    {
        public Queue<ActionBase> Plan(
            List<ActionStrategy> availableStrategies, 
            IEnumerable<Fact> worldState, 
            GoalBase goal)
        {
            HashSet<IObjectForAI> potentialTargets = new HashSet<IObjectForAI> { null };
            foreach (var condition in goal.GetDesiredState())
            {
                if (condition.SpecificObject != null)
                {
                    potentialTargets.Add(condition.SpecificObject);
                }
            }

            List<Fact> startState = new List<Fact>(worldState);
            float initialHeuristic = CalculateHeuristic(startState, goal);
            Node startNode = new Node(null, 0, initialHeuristic, startState, null, null);

            List<Node> openList = new List<Node> { startNode };
            
            int maxIterations = 50; 
            int iterations = 0;

            while (openList.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                
                openList = openList.OrderBy(n => n.TotalCost).ToList();
                Node current = openList[0];
                openList.RemoveAt(0);

                if (GoalAchieved(goal, current.State))
                {
                    return ConstructPlan(current);
                }

                foreach (ActionStrategy strategy in availableStrategies)
                {
                    foreach (IObjectForAI target in potentialTargets)
                    {
                        if (!CheckPreconditions(strategy, target, current.State))
                            continue;

                        List<Fact> nextState = new List<Fact>(current.State);
                        ApplyEffects(strategy, target, nextState);

                        float newCost = current.Cost + strategy.GetCost(target);
                        float newHeuristic = CalculateHeuristic(nextState, goal);

                        Node neighbor = new Node(current, newCost, newHeuristic, nextState, strategy, target);
                        openList.Add(neighbor);
                    }
                }
            }

            if (iterations >= maxIterations)
            {
                Debug.LogWarning("AI Planner: Превышен лимит итераций A* (возможно, цель недостижима).");
            }

            return null;
        }
        
        private float CalculateHeuristic(List<Fact> state, GoalBase goal)
        {
            float unmatchedConditions = 0;

            foreach (var condition in goal.GetDesiredState())
            {
                bool exists = state.Any(f => 
                    f.ConditionTag == condition.ConditionTag && 
                    (string.IsNullOrEmpty(condition.ObjectTag) || f.ObjectTags.Contains(condition.ObjectTag))
                );

                if (condition.MustExist && !exists)
                    unmatchedConditions++;
                else if (!condition.MustExist && exists)
                    unmatchedConditions++;
            }

            return unmatchedConditions; 
        }

        private Queue<ActionBase> ConstructPlan(Node goalNode)
        {
            List<ActionBase> result = new List<ActionBase>();
            Node n = goalNode;

            while (n != null)
            {
                if (n.Strategy != null)
                {
                    result.Insert(0, n.Strategy.CreateAction(n.Target));
                }
                n = n.Parent;
            }

            return new Queue<ActionBase>(result);
        }

        private bool CheckPreconditions(ActionStrategy strategy, IObjectForAI target, List<Fact> state)
        {
            foreach (GoalCondition condition in strategy.GetPreconditions(target))
            {
                bool exists = state.Any(f => 
                    f.ConditionTag == condition.ConditionTag && 
                    (string.IsNullOrEmpty(condition.ObjectTag) || f.ObjectTags.Contains(condition.ObjectTag))
                );

                if (condition.MustExist && !exists) return false;
                if (!condition.MustExist && exists) return false;
            }
            return true;
        }

        private void ApplyEffects(ActionStrategy strategy, IObjectForAI target, List<Fact> state)
        {
            foreach (GoalCondition effect in strategy.GetEffects(target))
            {
                if (effect.MustExist)
                {
                    bool exists = state.Any(f => 
                        f.ConditionTag == effect.ConditionTag && 
                        (string.IsNullOrEmpty(effect.ObjectTag) || f.ObjectTags.Contains(effect.ObjectTag))
                    );
                    
                    if (!exists)
                    {
                        state.Add(new Fact(effect.ConditionTag, effect.ObjectTag));
                    }
                }
                else
                {
                    state.RemoveAll(f => 
                        f.ConditionTag == effect.ConditionTag && 
                        (string.IsNullOrEmpty(effect.ObjectTag) || f.ObjectTags.Contains(effect.ObjectTag))
                    );
                }
            }
        }

        private bool GoalAchieved(GoalBase goal, List<Fact> state)
        {
            return CalculateHeuristic(state, goal) == 0;
        }
    }
}