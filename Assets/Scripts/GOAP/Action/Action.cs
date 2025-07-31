using System;

namespace GOAP
{
    public abstract class Action
    {
        protected abstract Condition[] Conditions { get; }
        protected abstract Effect[] Effects { get; }
        
        public Action IsComplete { get; set; }

        public abstract void Do();
    }
}