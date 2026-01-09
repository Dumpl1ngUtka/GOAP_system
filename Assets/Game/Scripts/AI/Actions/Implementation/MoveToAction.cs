using AI.Goal;
using AI.Knowledge;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions.Implementation
{
    public class MoveToAction : ActionBase
    {
        private NavMeshAgent _agent;
        private const float StoppingDistance = 1.5f;

        public MoveToAction(
            Transform transform, 
            NavMeshAgent agent) : base(transform)
        {
            _agent = agent;
            ActionName = "Move To";
            Cost = 1.0f;
        }

        protected override void DefinePreconditionsAndEffects()
        {
            _preconditions.Clear();
            _effects.Clear();

            if (Target != null)
            {
                foreach (string tag in Target.GetTags())
                {
                    _effects.Add(new GoalCondition(GlobalKeys.ConditionTag.Nearby, tag, true, Target));
                }
            }
        }

        public override void OnStart()
        {
            if (Target is IWorldObjectForAI worldObject)
            {
                _agent.isStopped = false;
                _agent.SetDestination(worldObject.GetWorldTransform().position);
            }
        }

        public override bool Perform(float deltaTime)
        {
            if (_agent.pathPending) return false;

            if (_agent.remainingDistance <= StoppingDistance)
            {
                return true; 
            }

            if (Target is IWorldObjectForAI worldObject)
            {
                _agent.SetDestination(worldObject.GetWorldTransform().position);
            }

            return false;
        }

        public override void OnStop()
        {
            _agent.isStopped = true;
        }
    }
}