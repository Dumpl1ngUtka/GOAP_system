using System;
using System.Collections.Generic;
using GOAP.Action;
using GOAP.KnowledgeBase;

namespace GOAP.Planner
{
    internal class Node
    {
        public Node Parent { get; }
        public float RunningCost { get; }
        public IGoapAction Action { get; }
        public List<Fact> WorldState { get; }

        public Node(Node parent, IGoapAction action, float runningCost, List<Fact> currentWorldState)
        {
            Parent = parent;
            RunningCost = runningCost;
            Action = action;
            WorldState = currentWorldState;
        }
    }
}