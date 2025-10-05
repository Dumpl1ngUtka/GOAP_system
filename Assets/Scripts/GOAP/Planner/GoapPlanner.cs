using System;
using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;

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

        private bool BuildGraph(Node parent, List<Node> leaves, List<IGoapAction> actions, List<Fact> goal)
        {
            var foundPath = false;

            foreach (var action in actions)
            {
                if (!IsActionUsable(action, parent.WorldState))
                {
                    continue;
                }

                var currentState = ApplyActionEffects(parent.WorldState, action.Effects.ToList());
                var node = new Node(parent,  action, parent.RunningCost + action.Cost, currentState);

                if (IsGoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
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

        private List<Fact> ApplyActionEffects(List<Fact> currentState, List<ActionWithFact> effects)
        {
            var newState = new List<Fact>(currentState);
        
            foreach (var actionEffect in effects)
            {
                switch (actionEffect.Type)
                {
                    case ActionType.None:
                        break;
                    case ActionType.Remove:
                        newState.Remove(actionEffect.Fact);
                        break;
                    case ActionType.Add:
                        newState.Add(actionEffect.Fact);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return newState;
        }

        private bool IsGoalAchieved(List<Fact> goal, List<Fact> currentState)
        {
            return goal.All(currentState.Contains);
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