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
        
        public Belief GetBelief(BeliefsList beliefKey)
        {
            foreach (var belief in _beliefs.Where(belief => belief.Key == beliefKey))
                return belief;

            return new Belief(BeliefsList.None);
        }

        public Belief[] GetAllBeliefs()
        {
            return _beliefs.ToArray();
        }
    }
}