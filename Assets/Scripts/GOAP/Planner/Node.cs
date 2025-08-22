using System.Collections.Generic;
using GOAP.Action;

namespace GOAP.Planner
{
    internal class Node
    {
        public Node Parent { get; }
        public float RunningCost { get; }
        public IGoapAction Action { get; }
        public Dictionary<string, object> State { get; }
        public Dictionary<string, object> Effects { get; }

        public Node(
            Node parent,
            float runningCost,
            IGoapAction action,
            Dictionary<string, object> state
        )
        {
            Parent = parent;
            RunningCost = runningCost;
            Action = action;
            State = new Dictionary<string, object>(state);
            Effects = action?.Effects ?? new Dictionary<string, object>();
        }
    }
}