using System.Collections.Generic;
using AI.Actions;
using AI.Knowledge;

namespace AI.Planner
{
    public class Node
    {
        public Node Parent;
        public float Cost;       
        public float Heuristic;  
        public List<Fact> State;
        public ActionStrategy Strategy;
        public IObjectForAI Target;
        
        public float TotalCost => Cost + Heuristic;

        public Node(Node parent, float cost, float heuristic, List<Fact> state, ActionStrategy strategy, IObjectForAI target)
        {
            Parent = parent;
            Cost = cost;
            Heuristic = heuristic;
            State = state;
            Strategy = strategy;
            Target = target;
        }
    }
}