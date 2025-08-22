using GOAP.KnowledgeBase;

namespace GOAP
{
    public interface IGoapGoal
    {
        string Name { get; }
        float Priority { get; }
        bool IsValid(IGoapKnowledge knowledge);
        void OnGoalActivated();
        void OnGoalDeactivated();
        System.Collections.Generic.Dictionary<string, object> GetDesiredState();
    }
}