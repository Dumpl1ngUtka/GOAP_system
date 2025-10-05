using GOAP.KnowledgeBase;
using Unit;
using UnityEngine;

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

        protected override void InitializeDesiredState()
        {
            AddDesiredEffect(FactTag.IsLowHealth, false);
            AddDesiredEffect(FactTag.IsInDangerous, true);
        }

        public override float GetPriority(IGoapKnowledge knowledge)
        {
            throw new System.NotImplementedException();
        }

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
    }
}