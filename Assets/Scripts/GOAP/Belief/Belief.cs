
using UnityEngine;

namespace GOAP
{
    public struct Belief
    {
        public readonly BeliefsList Key;
        public readonly int Value;
        public Vector3 Position;

        public Belief(BeliefsList key, int value = 1)
        {
            Key = key;
            Value = value;
            Position = Vector3.zero;
        }

        public Belief WithPosition(Vector3 position)
        {
            Position = position;
            return this;
        }
    }
}