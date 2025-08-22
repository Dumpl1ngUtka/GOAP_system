using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
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
            var startNode = new Node(null, 0, null, startState);

            var success = BuildGraph(startNode, leaves, actions, goalState);

            if (!success)
            {
                return false;
            }

            var cheapestNode = FindCheapestNode(leaves);
            plan = BuildActionQueue(cheapestNode);

            return plan.Count > 0;
        }

        private Dictionary<string, object> GetCurrentState(IGoapKnowledge knowledge)
        {
            var state = new Dictionary<string, object>();
            var facts = knowledge.GetAllFacts();
    
            foreach (var fact in facts)
            {
                state[fact.Key] = fact.Value;
            }

            return state;
        }

        private bool BuildGraph(
            Node parent,
            List<Node> leaves,
            List<IGoapAction> actions,
            Dictionary<string, object> goal
        )
        {
            var foundPath = false;

            foreach (var action in actions)
            {
                if (!IsActionUsable(action, parent.State))
                {
                    continue;
                }

                var currentState = ApplyActionEffects(parent.State, action.Effects);
                var node = new Node(parent, parent.RunningCost + action.Cost, action, currentState);

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

        private bool IsActionUsable(IGoapAction action, Dictionary<string, object> state)
        {
            foreach (var precondition in action.Preconditions)
            {
                if (!state.ContainsKey(precondition.Key))
                {
                    return false;
                }

                if (!state[precondition.Key].Equals(precondition.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, object> ApplyActionEffects(
            Dictionary<string, object> currentState,
            Dictionary<string, object> effects
        )
        {
            var newState = new Dictionary<string, object>(currentState);
        
            foreach (var effect in effects)
            {
                newState[effect.Key] = effect.Value;
            }

            return newState;
        }

        private bool IsGoalAchieved(Dictionary<string, object> goal, Dictionary<string, object> state)
        {
            foreach (var condition in goal)
            {
                if (!state.ContainsKey(condition.Key))
                {
                    return false;
                }

                if (!state[condition.Key].Equals(condition.Value))
                {
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