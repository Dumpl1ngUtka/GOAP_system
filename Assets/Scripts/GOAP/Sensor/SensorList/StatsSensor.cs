using System.Collections.Generic;
using Unit.Stats;

namespace GOAP.SensorList
{
    public class StatsSensor : ISensor
    {
        private Stats _unitStats;
        
        public StatsSensor(Stats unitStats)
        {
            _unitStats = unitStats;
        }
        
        public Belief[] GetBeliefs()
        {
            var beliefs = new List<Belief>();
            if (_unitStats.CurrentHealth < _unitStats.MaxHealth / 2)
                beliefs.Add(new Belief(BeliefsList.LowHealth));
            return beliefs.ToArray();
        }
    }
}