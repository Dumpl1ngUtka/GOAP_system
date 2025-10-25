using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;
using GOAP.Planner;
using GOAP.Sensor;
using TEST;
using Unit;
using Unit.Mover;
using UnityEngine;

namespace GOAP.Agent
{
    public interface IGoapAgent
    {
        int TeamId { get; }
        Transform GetTransform { get; }
        void Initialize(
            int teamId,
            IGoapKnowledge knowledge, 
            IGoapPlanner planner, 
            IAgentMover mover, 
            IPlanContainer planContainer,
            IHealth health,
            AgentsInfoHolder<GoapAgent> enemyInfoHolder,
            AgentsInfoHolder<GoapAgent> alliesInfoHolder,
            params IGoapSensor[] sensors);
        
        void AddGoal(IGoapGoal goal);
        void RemoveGoal(string goalName);
        void AddAction(IGoapAction action);

    }
}