using System.Collections.Generic;
using System.Linq;

namespace GOAP
{
    public class BeliefsHolder
    {
        private List<Belief> _beliefs;

        public void SetBeliefs(List<Belief> belief)
        {
            _beliefs = belief;
        }
        
        public Belief GetBelief(string beliefName)
        {
            foreach (var belief in _beliefs.Where(belief => belief.Name == beliefName))
                return belief;

            return new Belief("");
        }
    }
}