using System.Collections.Generic;
using GOAP.KnowledgeBase;

namespace GOAP.Goal
{
    public interface IGoapGoal
    {
        string Name { get; }
        float GetPriority(IGoapKnowledge knowledge);
        bool IsValid(IGoapKnowledge knowledge);
        void OnGoalActivated();
        void OnGoalDeactivated();
        IEnumerable<Fact> GetDesiredState();
    }
}