using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Planner
{
    public class GoapPlanner : IGoapPlanner
    {
        public Queue<IGoapAction> Plan(
            IGoapKnowledge knowledge,
            IGoapGoal goal,
            IEnumerable<IGoapAction> availableActions
        )
        {
            if (TryPlan(knowledge, goal, availableActions, out var plan))
            {
                return plan;
            }

            return new Queue<IGoapAction>();
        }

        public bool TryPlan(
            IGoapKnowledge knowledge,
            IGoapGoal goal,
            IEnumerable<IGoapAction> availableActions,
            out Queue<IGoapAction> plan
        )
        {
            plan = new Queue<IGoapAction>();
            var actions = availableActions.ToList();
        
            if (actions.Count == 0)
            {
                return false;
            }

            var startState = GetCurrentState(knowledge);
            var goalState = goal.GetDesiredState();

            var leaves = new List<Node>();
            var startNode = new Node(null,  null, 0, startState);

            var success = BuildGraph(startNode, leaves, actions, goalState.ToList());

            if (!success)
            {
                return false;
            }

            var cheapestNode = FindCheapestNode(leaves);
            plan = BuildActionQueue(cheapestNode);

            return plan.Count > 0;
        }

        private List<Fact> GetCurrentState(IGoapKnowledge knowledge) => 
            knowledge.GetAllFacts().ToList();

        private bool BuildGraph(Node parent, List<Node> leaves, List<IGoapAction> actions, List<FactWithCondition> goal)
        {
            var foundPath = false;

            foreach (var action in actions)
            {
                Debug.Log("Check action for plan: " + action + " with current plan ");
                foreach (var node1 in leaves)
                {
                    Debug.Log(node1);
                }
                if (!IsActionUsable(action, parent.WorldState))
                {
                    Debug.Log("cant be usable");
                    continue;
                }


                var currentState = ApplyActionEffects(parent.WorldState, action.GetEffects().ToList());
                var node = new Node(parent,  action, parent.RunningCost + action.Cost, currentState);
                leaves.Add(node);

                if (IsGoalAchieved(goal, currentState))
                {
                    Debug.Log("Achieved goal");
                    foundPath = true;
                }
                else
                {
                    var subset = actions.Where(a => a != action).ToList();
                    foundPath = BuildGraph(node, leaves, subset, goal) || foundPath;
                }
            }

            return foundPath;
        }

        private bool IsActionUsable(IGoapAction action, List<Fact> state) => 
            action.CheckProceduralPrecondition(state);

        private List<Fact> ApplyActionEffects(List<Fact> currentState, List<FactWithCondition> effects)
        {
            var newState = new List<Fact>(currentState);
        
            foreach (var actionEffect in effects)
            {
                switch (actionEffect.Type)
                {
                    case FactCondition.None:
                        break;
                    case FactCondition.Exclude:
                        newState.Remove(actionEffect.Fact);
                        break;
                    case FactCondition.Include:
                        newState.Add(actionEffect.Fact);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return newState;
        }

        private bool IsGoalAchieved(List<FactWithCondition> goal, List<Fact> currentState)
        {
            foreach (var fact in goal)
            {
                switch (fact.Type)
                {
                    case FactCondition.Include when currentState.Contains(fact.Fact):
                    case FactCondition.Exclude when !currentState.Contains(fact.Fact):
                        continue;
                    case FactCondition.None:
                        Debug.LogError("Fact condition not set");
                        return false;
                    default:
                        return false;
                }
            }
            return true;
        }

        private Node FindCheapestNode(List<Node> leaves)
        {
            Node cheapest = null;
        
            foreach (var leaf in leaves)
            {
                if (cheapest == null || leaf.RunningCost < cheapest.RunningCost)
                {
                    cheapest = leaf;
                }
            }

            return cheapest;
        }

        private Queue<IGoapAction> BuildActionQueue(Node node)
        {
            var queue = new Queue<IGoapAction>();
            var actions = new List<IGoapAction>();

            while (node != null && node.Action != null)
            {
                actions.Insert(0, node.Action);
                node = node.Parent;
            }

            foreach (var action in actions)
            {
                queue.Enqueue(action);
            }

            return queue;
        }
    }
}