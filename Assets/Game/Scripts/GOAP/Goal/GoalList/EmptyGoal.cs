using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;

namespace GOAP.Goal.GoalList
{
    public class EmptyGoal : GoapGoal
    {
        public override string Name => "EmptyGoal";
        public override IObjectForFact GetTarget() => null;

        public override float GetPriority(IGoapKnowledge knowledge) => 0.01f;

        public override bool IsValid(IGoapKnowledge knowledge) => true;
        public override void OnGoalActivated()
        {
        }

        public override void OnGoalDeactivated()
        {
        }

        public override IEnumerable<FactWithCondition> GetDesiredState() => new List<FactWithCondition>();
    }
}