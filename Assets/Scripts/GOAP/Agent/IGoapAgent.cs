using GOAP.Action;
using GOAP.Goal;
using UnityEngine;

namespace GOAP.Agent
{
    public interface IGoapAgent
    {
        Transform GetTransform { get; }
        void AddGoal(IGoapGoal goal);
        void RemoveGoal(string goalName);
        void AddAction(IGoapAction action);

    }
}