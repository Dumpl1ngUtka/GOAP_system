using System.Collections.Generic;
using GOAP.Action;
using GOAP.Agent;
using GOAP.KnowledgeBase;
using GOAP.Sensor;
using TEST;
using UnityEngine;

namespace GOAP.Goal.GoalList
{
    public class SurviveGoal : GoapGoal
    {
        private const float PriorityPerEnemy = 5;
        private AgentsInfoHolder<GoapAgent> _enemyInfoHolder;
        
        public override string Name => "SurviveGoal";
        public override IObjectForFact GetTarget()
        {
            return _enemyInfoHolder.GetNearestAgent(Vector3.zero);
        }

        public SurviveGoal(AgentsInfoHolder<GoapAgent> enemyInfoHolder)
        {
            _enemyInfoHolder = enemyInfoHolder;
        }

        public override float GetPriority(IGoapKnowledge knowledge)
        {

            return 5;
            var priority = _enemyInfoHolder.GetAgentsCount() * PriorityPerEnemy;
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
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.IsLowHealth)),
            };
        }
    }
}