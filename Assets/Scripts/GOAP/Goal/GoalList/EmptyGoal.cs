using GOAP.KnowledgeBase;

namespace GOAP.Goal.GoalList
{
    public class EmptyGoal : GoapGoal
    {
        public override string Name => "EmptyGoal";
        protected override void InitializeDesiredState()
        {
        }

        public override float GetPriority(IGoapKnowledge knowledge)
        {
            return 0.01f;
        }

        public override bool IsValid(IGoapKnowledge knowledge)
        {
            return true;
        }
    }
}