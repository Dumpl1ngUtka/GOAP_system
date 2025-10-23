using System.Collections.Generic;
using GOAP.Action;
using GOAP.Agent;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using TEST;
using UnityEngine;

namespace GOAP.Goal.GoalList
{
    public class EliminateGoal : GoapGoal
    {
        private AgentsInfoHolder<Enemy> _enemyInfoHolder;
        public override string Name => "EliminateGoal";

        public EliminateGoal(AgentsInfoHolder<Enemy> enemyInfoHolder)
        {
            _enemyInfoHolder = enemyInfoHolder;
        }
        
        public override IObjectForFact GetTarget() => _enemyInfoHolder.GetNearestAgent(Vector3.zero);

        public override float GetPriority(IGoapKnowledge knowledge) => 2f;

        public override bool IsValid(IGoapKnowledge knowledge)
        {
            return knowledge.ContainsFactWithObjectTag(FactTag.Nearby, ObjectForFactTag.Enemy) ||
                   knowledge.ContainsFactWithObjectTag(FactTag.Around, ObjectForFactTag.Enemy);
        }

        public override void OnGoalActivated()
        {
            
        }

        public override void OnGoalDeactivated()
        {
            
        }

        public override IEnumerable<FactWithCondition> GetDesiredState()
        {
            return new[]
            {
                new FactWithCondition(
                    FactCondition.Exclude, 
                    new Fact(FactTag.Nearby).WithAdditionObjectForFactTags(ObjectForFactTag.Enemy)),
                
                new FactWithCondition(
                    FactCondition.Exclude,
                    new Fact(FactTag.Around).WithAdditionObjectForFactTags(ObjectForFactTag.Enemy))
            };
        }
    }
}