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
        
        private readonly string _beliefName;
        private readonly Type _operationType;
        private readonly int _value;

        public Condition(string beliefName, Type operationType, int value)
        {
            _beliefName = beliefName;
            _operationType = operationType;
            _value = value;
        }

        public bool IsValid(Agent agent)
        { 
            if (agent.TryGetBelief(_beliefName, out var belief))
                return false;

            var beliefCount = belief.Count;

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