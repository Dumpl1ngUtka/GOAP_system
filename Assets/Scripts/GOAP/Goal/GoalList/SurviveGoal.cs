using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using GOAP.Sensor;
using Unit;

namespace GOAP.Goal.GoalList
{
    public class SurviveGoal : GoapGoal
    {
        private const float PriorityPerEnemy = 5;
        private EnemyInfoHolder _enemyInfoHolder;
        
        public override string Name => "SurviveGoal";
        public override IObjectForFact GetTarget() => _enemyInfoHolder.GetNearestEnemy(); 


        public SurviveGoal(EnemyInfoHolder enemyInfoHolder)
        {
            _enemyInfoHolder = enemyInfoHolder;
        }

        public override float GetPriority(IGoapKnowledge knowledge)
        {
            var priority = _enemyInfoHolder.GetEnemiesCount() * PriorityPerEnemy;
            if (knowledge.ContainsFact(FactTag.IsLowHealth))
                priority *= 3;
            return priority;
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

        public override IEnumerable<FactWithCondition> GetDesiredState()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.IsInDangerous)),
            };
        }
    }
}