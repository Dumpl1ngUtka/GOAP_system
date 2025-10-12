using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;
using GOAP.Planner;

namespace GOAP.Goal
{
    public interface IGoapGoal
    {
        string Name { get; }
        IGoapGoal Init(IPlanContainer planContainer);
        float GetPriority(IGoapKnowledge knowledge);
        bool IsValid(IGoapKnowledge knowledge);
        void OnGoalActivated();
        void OnGoalDeactivated();
        IEnumerable<FactWithCondition> GetDesiredState();
    }
}