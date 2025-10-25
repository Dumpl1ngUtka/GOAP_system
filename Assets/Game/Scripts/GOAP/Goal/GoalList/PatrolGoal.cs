using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;

namespace GOAP.Goal.GoalList
{
    public class PatrolGoal : GoapGoal
    {
        public override string Name => "PatrolGoal";
        public override IObjectForFact GetTarget() => null;

        public override float GetPriority(IGoapKnowledge knowledge) => 1f;

        public override bool IsValid(IGoapKnowledge knowledge) => true;
        
        public override void OnGoalActivated() { }

        public override void OnGoalDeactivated() { }

        public override IEnumerable<FactWithCondition> GetDesiredState()
        {
            return new []
            {
                new FactWithCondition(FactCondition.Include, new Fact(FactTag.IsPatrolling)),
            };
        }
        
        
    }
}