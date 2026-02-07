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
            List<Node> leaves = new List<Node>();
            Node startNode = new Node(null, 0, new List<Fact>(worldState), null, null);
            
            // We need to collect potential targets.
            // Since Fact doesn't store the object reference, we rely on the Goal's SpecificObject 
            // or assume strategies can work with null targets for now, 
            // OR we assume the caller might provide a list of relevant objects (not in current signature).
            
            // However, to make it work with the current structure:
            // We can extract potential targets from the Goal conditions if they have SpecificObject set.
            HashSet<IObjectForAI> potentialTargets = new HashSet<IObjectForAI>();
            potentialTargets.Add(null); // Always consider 'no target' (self)

            foreach (var condition in goal.GetDesiredState())
            {
                if (condition.SpecificObject != null)
                {
                    potentialTargets.Add(condition.SpecificObject);
                }
            }

            bool success = BuildGraph(startNode, leaves, availableStrategies, goal, potentialTargets);

            if (!success)
            {
                return null;
            }

            Node cheapestNode = null;
            foreach (Node leaf in leaves)
            {
                if (cheapestNode == null || leaf.Cost < cheapestNode.Cost)
                {
                    cheapestNode = leaf;
                }
            }

            List<ActionBase> result = new List<ActionBase>();
            Node n = cheapestNode;
            while (n != null)
            {
                if (n.Strategy != null)
                {
                    result.Insert(0, n.Strategy.CreateAction(n.Target));
                }
                n = n.Parent;
            }

            Queue<ActionBase> queue = new Queue<ActionBase>();
            foreach (var a in result)
            {
                queue.Enqueue(a);
            }
            return queue;
        }

        private bool BuildGraph(
            Node parent, 
            List<Node> leaves, 
            List<ActionStrategy> strategies, 
            GoalBase goal,
            HashSet<IObjectForAI> potentialTargets)
        {
            bool found = false;

            foreach (var strategy in strategies)
            {
                foreach (var target in potentialTargets)
                {
                    if (!CheckPreconditions(strategy, target, parent.State))
                        continue;

                    List<Fact> currentState = new List<Fact>(parent.State);
                    ApplyEffects(strategy, target, currentState);

                    Node node = new Node(parent, parent.Cost + strategy.GetCost(target), currentState, strategy, target);

                    if (GoalAchieved(goal, currentState))
                    {
                        leaves.Add(node);
                        found = true;
                    }
                    else
                    {
                        // Simple depth limit or cycle check could be added here
                        if (node.Cost < 100) // Arbitrary cost limit to prevent infinite loops
                        {
                            if (BuildGraph(node, leaves, strategies, goal, potentialTargets))
                                found = true;
                        }
                    }
                }
            }

            return found;
        }

        private bool CheckPreconditions(ActionStrategy strategy, IObjectForAI target, List<Fact> state)
        {
            foreach (var condition in strategy.GetPreconditions(target))
            {
                // Check if the condition is satisfied by the current state (facts)
                bool exists = state.Any(f => 
                    f.ConditionTag == condition.ConditionTag && 
                    (string.IsNullOrEmpty(condition.ObjectTag) || f.ObjectTags.Contains(condition.ObjectTag))
                );

                if (condition.MustExist && !exists)
                    return false;
                
                if (!condition.MustExist && exists)
                    return false;
            }
            return true;
        }

        private void ApplyEffects(ActionStrategy strategy, IObjectForAI target, List<Fact> state)
        {
            foreach (var effect in strategy.GetEffects(target))
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
            foreach (var condition in goal.GetDesiredState())
            {
                bool exists = state.Any(f => 
                    f.ConditionTag == condition.ConditionTag && 
                    (string.IsNullOrEmpty(condition.ObjectTag) || f.ObjectTags.Contains(condition.ObjectTag))
                );

                if (condition.MustExist && !exists)
                    return false;
                
                if (!condition.MustExist && exists)
                    return false;
            }
            return true;
        }
    }
}