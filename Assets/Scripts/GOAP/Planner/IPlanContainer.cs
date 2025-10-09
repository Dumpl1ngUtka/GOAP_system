using GOAP.KnowledgeBase;

namespace GOAP.Planner
{
    public interface IPlanContainer
    {
        void SetTarget(IObjectForFact target);
        IObjectForFact GetTarget();
    }
}