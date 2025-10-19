using System.Collections.Generic;
using GOAP.Action;
using GOAP.Agent;
using GOAP.KnowledgeBase;
using GOAP.Planner;

namespace GOAP.Goal.GoalList
{
    public class EliminateGoal : GoapGoal
    {
        private EnemyInfoHolder _enemyInfoHolder;
        public override string Name => "EliminateGoal";

        public EliminateGoal(EnemyInfoHolder enemyInfoHolder)
        {
            _enemyInfoHolder = enemyInfoHolder;
        }
        
        public override IObjectForFact GetTarget() => _enemyInfoHolder.GetNearestEnemy();

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
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.Nearby, new List<ObjectForFactTag>()
                {
                    ObjectForFactTag.Enemy
                })),
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.Around, new List<ObjectForFactTag>()
                {
                    ObjectForFactTag.Enemy
                }))
            };
        }
    }
}