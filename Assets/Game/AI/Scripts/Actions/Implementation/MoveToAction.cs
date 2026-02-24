using AI.Knowledge;
using Units.Mover;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class MoveToAction : ActionBase
    {
        private readonly AgentMover _mover;
        
        public MoveToAction(
            IObjectForAI target, 
            AgentMover mover) : base(target)
        {
            _mover = mover;
        }
        
        public override void OnStart()
        {
            Debug.Log("Start Move with target " + Target);
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