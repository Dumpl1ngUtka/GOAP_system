using System;

namespace GOAP
{
    public readonly struct Condition
    {
        public enum Type
        {
            Contains,
            Less,
            LessOrEqual,
            Equal,
            NotEqual,
            GreaterOrEqual,
            Greater,
        }
        
        private readonly BeliefsList _beliefKey;
        private readonly Type _operationType;
        private readonly int _value;

        public Condition(BeliefsList beliefKey, Type operationType, int value)
        {
            _beliefKey = beliefKey;
            _operationType = operationType;
            _value = value;
        }

        public bool IsValid(Agent agent)
        { 
            if (agent.TryGetBelief(_beliefKey, out var belief))
                return false;

            var beliefCount = belief.Value;

            return _operationType switch
            {
                Type.Contains => true,
                Type.Less => beliefCount < _value,
                Type.LessOrEqual => beliefCount <= _value,
                Type.Equal => beliefCount == _value,
                Type.NotEqual => beliefCount != _value,
                Type.GreaterOrEqual => beliefCount >= _value,
                Type.Greater => beliefCount > _value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}