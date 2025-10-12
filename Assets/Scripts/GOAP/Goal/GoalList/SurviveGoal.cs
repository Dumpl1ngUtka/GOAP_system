using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using Unit;

namespace GOAP.Goal.GoalList
{
    public class SurviveGoal : GoapGoal
    {
        public override string Name => "SurviveGoal";

        private readonly IHealth _health;

        public SurviveGoal(IHealth health)
        {
            _health = health;
        }

        public override float GetPriority(IGoapKnowledge knowledge) => 10f;

        public override bool IsValid(IGoapKnowledge knowledge)
        {
            return knowledge.ContainsFact(FactTag.IsLowHealth);
        }

        public override void OnGoalActivated()
        {
            //Debug.Log("SurviveGoal activated: Health is low!");
        }

        public override void OnGoalDeactivated()
        {
            //Debug.Log("SurviveGoal deactivated: No longer in danger.");
        }

        public override IEnumerable<FactWithCondition> GetDesiredState()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.IsLowHealth, PlanContainer.GetTarget())),
            };
        }
    }
}