using AI.Knowledge;

namespace AI.Actions
{
    public abstract class ActionBase
    {
        protected readonly IObjectForAI Target;

        protected ActionBase(IObjectForAI target)
        {
            Target = target;
        }
        
        public virtual void OnStart() { }
        public abstract bool Perform(float deltaTime);
        public virtual void OnStop() { }
    }
}