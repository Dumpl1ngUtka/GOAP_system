using AI.Goal;
using AI.Knowledge;
using Units.Mover;
using UnityEngine;
using UnityEngine.AI;

namespace AI.Actions.Implementation
{
    public class MoveToAction : ActionBase
    {
        private readonly AgentMover _mover;
        
        public MoveToAction(
            Transform transform, 
            AgentMover mover) : base(transform)
        {
            _mover = mover;
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
                _mover.SetTargetPosition(worldObject.GetWorldTransform().position);
            }
        }

        public override bool Perform(float deltaTime)
        {
            return _mover.IsMoving;
        }

        public override void OnStop()
        {
            _mover.Stop();
        }
    }
}