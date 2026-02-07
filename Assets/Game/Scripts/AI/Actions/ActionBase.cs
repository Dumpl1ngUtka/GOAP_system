using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Actions
{
    public abstract class ActionBase
    {
        public float Cost = 1.0f;
        
        protected readonly Transform Transform;
        protected IObjectForAI Target; 

        protected readonly List<GoalCondition> _preconditions = new List<GoalCondition>();
        protected readonly List<GoalCondition> _effects = new List<GoalCondition>();

        public IEnumerable<GoalCondition> Preconditions => _preconditions;
        public IEnumerable<GoalCondition> Effects => _effects;

        public ActionBase(Transform transform)
        {
            Transform = transform;
        }
        
        public virtual void Setup(IObjectForAI target)
        {
            Target = target;
            DefinePreconditionsAndEffects();
        }
        
        protected abstract void DefinePreconditionsAndEffects();
        
        public virtual bool IsValid() => true;
        
        public virtual void OnStart() { }
        
        public abstract bool Perform(float deltaTime);

        public virtual void OnStop() { }
    }
}