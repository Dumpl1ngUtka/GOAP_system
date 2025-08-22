using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;

namespace GOAP.Planner
{
    public interface IGoapPlanner
    {
        Queue<IGoapAction> Plan(
            IGoapKnowledge knowledge,
            IGoapGoal goal,
            List<IGoapAction> availableActions
        );
    }
}