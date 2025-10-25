using System.Collections.Generic;
using System.Linq;
using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using UnityEngine;

namespace GOAP.Planner
{
    public class CustomGoapPlanner : IGoapPlanner
    {
        public Queue<IGoapAction> Plan(IGoapKnowledge knowledge, IGoapGoal goal, IEnumerable<IGoapAction> availableActions)
        {
            return TryPlan(knowledge, goal, availableActions, out var plan) ? plan : null;
        }

        public bool TryPlan(IGoapKnowledge knowledge, IGoapGoal goal, IEnumerable<IGoapAction> availableActions, out Queue<IGoapAction> plan)
        {
            plan = null;
            var worldState = new WorldState(knowledge.GetAllFacts(), goal.GetTarget());
            var availableActionsList = new List<IGoapAction>();
            foreach (var action in availableActions)
                availableActionsList.Add(action.Clone().WithTarget(goal.GetTarget()));
            
            Debug.Log("PLAN FOR GOAL " + goal.Name + " WITH TARGET " + goal.GetTarget());

            var queue = new Queue<SearchNode>();
            queue.Enqueue(new SearchNode(worldState.GetClone(), new List<IGoapAction>()));

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                if (node.State.IsGoalAchieved(goal))
                {
                    Debug.Log("GOAL "+ goal.Name + " ACHIEVED");
                    plan = new Queue<IGoapAction>(node.Plan);
                    return true;
                }

                if (node.Plan.Count >= 5)
                    continue;

                foreach (var action in availableActionsList)
                {
                    var newState = node.State.GetClone();
                    if (!action.CheckProceduralPrecondition(newState.Facts))
                        continue;

                    newState.AddEffects(action.GetEffects());

                    var newPlan = new List<IGoapAction>(node.Plan);
                    newPlan.Add(action);

                    queue.Enqueue(new SearchNode(newState, newPlan));
                }
            }

            return false;
        }

        private class SearchNode
        {
            public WorldState State { get; }
            public List<IGoapAction> Plan { get; }

            public SearchNode(WorldState state, List<IGoapAction> plan)
            {
                State = state;
                Plan = plan;

                var facts = "";
                var plans = "";
                foreach (var fact in state.Facts)
                {
                    var a = "";
                    foreach (var dad in fact.ObjectTags)
                    {
                        a  += dad + " ";
                    }

                    facts += fact.Tag + "{" + a + "}" + " ";
                }

                foreach (var plan1 in plan)
                {
                    plans += plan1.Name + "->";
                }
                
                Debug.Log("Facts = " + facts + " For Plan = " + plans);
            }
        }
    }

    public class WorldState
    {
        public List<Fact> Facts { get; private set; }
        public IObjectForFact TargetObject { get; set; }
        
        public WorldState(IEnumerable<Fact> facts, IObjectForFact targetObject)
        {
            Facts = facts.ToList();
            TargetObject = targetObject;
        }

        public void AddEffects(IEnumerable<FactWithCondition> factsWithConditions)
        {
            foreach (var factWithCondition in factsWithConditions)
            {
                if (!CheckFactWithCondition(factWithCondition, out var fact))
                {
                    if (factWithCondition.Type == FactCondition.Include)
                        Facts.Add(factWithCondition.Fact);
                    else
                    {
                        Facts.Remove(fact);
                    }
                }
            }
        }

        public bool IsGoalAchieved(IGoapGoal goal)
        {
            foreach (var goalFactWithCondition in goal.GetDesiredState())
            {
                if (!CheckFactWithCondition(goalFactWithCondition, out _))
                    return false;
            }
            return true;
        }

        public WorldState GetClone() => new(Facts, TargetObject);

        private bool CheckFactWithCondition(FactWithCondition factWithCondition, out Fact factInList)
        {
            var isNeedToBeInclude = factWithCondition.Type == FactCondition.Include;
            var has = false;
            factInList = null;
            foreach (var fact in Facts)
            {
                factInList = fact;
                if (fact.Tag != factWithCondition.Fact.Tag)
                    continue;

                var factHasAllTags = true;
                foreach (var factObjectTag in factWithCondition.Fact.ObjectTags)
                {
                    if (!fact.ObjectTags.Contains(factObjectTag))
                    {
                        factHasAllTags = false; 
                        break;
                    }
                }

                if (factHasAllTags)
                {
                    has = true;
                    break;
                }
            }
            return has && isNeedToBeInclude || !has && !isNeedToBeInclude;
        }
    }
}