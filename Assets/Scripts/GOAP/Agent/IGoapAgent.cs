using GOAP.Action;
using GOAP.Goal;

namespace GOAP.Agent
{
    public interface IGoapAgent
    {
        void AddGoal(IGoapGoal goal);
        void RemoveGoal(string goalName);
        void AddAction(IGoapAction action);

    }
}