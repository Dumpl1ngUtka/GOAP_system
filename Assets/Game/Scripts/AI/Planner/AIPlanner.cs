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
        private class PlanNode
        {
            public PlanNode Parent;
            public ActionBase Action;
            public IObjectForAI ActionTarget;
            
            public List<GoalCondition> RequiredState; 
            
            public float Cost;
            
            public PlanNode(PlanNode parent, ActionBase action, IObjectForAI target, List<GoalCondition> state, float cost)
            {
                Parent = parent;
                Action = action;
                ActionTarget = target;
                RequiredState = state;
                Cost = cost;
            }
        }
        
        public Queue<ActionBase> Plan(
            MonoBehaviour agent, 
            List<ActionBase> availableActions, 
            IEnumerable<Fact> worldKnowledge, 
            GoalBase goal)
        {
            List<GoalCondition> goalConditions = goal.GetDesiredState().ToList();
            List<Fact> currentWorldState = worldKnowledge.ToList();
            
            PlanNode startNode = new PlanNode(null, null, null, goalConditions, 0);
            
            List<PlanNode> openList = new List<PlanNode> { startNode };
            
            int iterations = 0;
            int maxIterations = 100; 

            while (openList.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                
                openList.Sort((a, b) => (a.Cost + a.RequiredState.Count).CompareTo(b.Cost + b.RequiredState.Count));
                PlanNode currentNode = openList[0];
                openList.RemoveAt(0);
                
                if (AreConditionsMet(currentNode.RequiredState, currentWorldState))
                {
                    return ReconstructPath(currentNode);
                }
                
                foreach (var action in availableActions)
                {
                    var satisfiableConditions = GetSatisfiableConditions(action, currentNode.RequiredState);

                    foreach (var condition in satisfiableConditions)
                    {
                        IObjectForAI target = condition.SpecificObject;
                        action.Setup(target); 
                        List<GoalCondition> newRequiredState = new List<GoalCondition>(currentNode.RequiredState);
                        
                        newRequiredState.RemoveAll(c => c.ConditionTag == condition.ConditionTag && c.ObjectTag == condition.ObjectTag);

                        foreach (var precond in action.Preconditions)
                        {
                            if (!newRequiredState.Any(existing => 
                                existing.ConditionTag == precond.ConditionTag && 
                                existing.ObjectTag == precond.ObjectTag && 
                                existing.SpecificObject == precond.SpecificObject))
                            {
                                newRequiredState.Add(precond);
                            }
                        }

                        float newCost = currentNode.Cost + action.Cost;
                        PlanNode neighbor = new PlanNode(currentNode, action, target, newRequiredState, newCost);
                        
                        openList.Add(neighbor);
                    }
                }
            }
            
            return null;
        }
        
        private bool AreConditionsMet(List<GoalCondition> conditions, List<Fact> worldState)
        {
            foreach (var cond in conditions)
            {
                bool factExists = worldState.Any(f => 
                    f.ConditionTag == cond.ConditionTag &&
                    f.ObjectTags == cond.ObjectTag &&
                    (cond.SpecificObject == null || f.Object == cond.SpecificObject)
                );
                
                if (factExists != cond.MustExist)
                {
                    return false;
                }
            }
            return true;
        }
        
        private List<GoalCondition> GetSatisfiableConditions(ActionBase action, List<GoalCondition> requiredState)
        {
            List<GoalCondition> result = new List<GoalCondition>();

            foreach (var req in requiredState)
            {
                if (req.SpecificObject != null)
                {
                    action.Setup(req.SpecificObject);
                }
                
                foreach (var effect in action.Effects)
                {
                    if (effect.ConditionTag == req.ConditionTag &&
                        effect.ObjectTag == req.ObjectTag &&
                        effect.MustExist == req.MustExist)
                    {
                        result.Add(req);
                        break; 
                    }
                }
            }
            
            return result;
        }
        
        private Queue<ActionBase> ReconstructPath(PlanNode node)
        {
            List<ActionBase> path = new List<ActionBase>();
            
            PlanNode current = node;
            while (current != null && current.Action != null)
            {
                current.Action.Setup(current.ActionTarget);
                
                path.Add(current.Action);
                current = current.Parent;
            }
            
            return new Queue<ActionBase>(path);
        }
    }
}