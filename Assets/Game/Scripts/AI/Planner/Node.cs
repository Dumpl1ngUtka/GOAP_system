using System.Collections.Generic;
using AI.Actions;
using AI.Knowledge;

namespace AI.Planner
{
    public class Node
    {
        public Node Parent;
        public float Cost;
        public List<Fact> State;
        public ActionStrategy Strategy;
        public IObjectForAI Target;

        public Node(Node parent, float cost, List<Fact> state, ActionStrategy strategy, IObjectForAI target)
        {
            Parent = parent;
            Cost = cost;
            State = state;
            Strategy = strategy;
            Target = target;
        }
    }
}