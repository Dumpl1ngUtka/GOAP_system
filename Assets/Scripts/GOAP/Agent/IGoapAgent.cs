using GOAP.Action;

namespace GOAP
{
    public interface IGoapAgent
    {
        void AddGoal(IGoapGoal goal);
        void RemoveGoal(string goalName);
        void AddAction(IGoapAction action);
        void Replan();
        void AbortCurrentPlan();
    }
}