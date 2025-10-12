using System.Collections.Generic;
using GOAP.Action;
using GOAP.Goal;
using GOAP.KnowledgeBase;

namespace GOAP.Planner
{
    public interface IPlanContainer
    {
        void SetPlan(Queue<IGoapAction> plan);
        Queue<IGoapAction> GetPlan();
        void ResetPlan();
        IGoapAction Dequeue();
        bool IsPlanValid();
        void SetTarget(IObjectForFact target);
        IObjectForFact GetTarget();
    }
}