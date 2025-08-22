using GOAP.KnowledgeBase;
using Unit;
using UnityEngine;

namespace GOAP.Goal.GoalList
{
    public class SurviveGoal : GoapGoal
    {
        public override string Name => "SurviveGoal";
        public override float Priority { get; }

        private readonly IHealth _health;

        public SurviveGoal(IHealth health)
        {
            _health = health;
        }

        protected override void InitializeDesiredState()
        {
            AddDesiredEffect("isHealthLow", false);
            AddDesiredEffect("isSafe", true);
        }

        public override bool IsValid(IGoapKnowledge knowledge)
        {
            if (_health == null) return false;
        
            return _health.IsHealthLow && 
                   knowledge.TryGetFact<bool>("isUnderAttack", out bool underAttack) && 
                   underAttack;
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