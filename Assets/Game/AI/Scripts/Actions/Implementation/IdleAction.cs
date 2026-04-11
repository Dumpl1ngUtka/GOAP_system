using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class IdleAction : ActionBase
    {
        private readonly float _stateTime = 3f;
        
        private float _timer;
        
        public IdleAction(IObjectForAI target) : base(target)
        {
        }

        public override void OnStart()
        {
            base.OnStart();
            _timer = 0;
        }

        public override bool Perform(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer > _stateTime)
            {
                Debug.Log("Idle action perform");
                return true;
            }
            return false;
        }
    }
}