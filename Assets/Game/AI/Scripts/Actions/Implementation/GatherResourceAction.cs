using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class GatherResourceAction : ActionBase
    {
        private float _gatherTimer;
        private readonly float _gatherDuration;
        private readonly Transform _selfTransform;

        public GatherResourceAction(
            Transform selfTransform,
            IObjectForAI target,
            float gatherDuration) : base(target)
        {
            _selfTransform = selfTransform;
            _gatherDuration = gatherDuration;
        }

        public override void OnStart()
        {
            _gatherTimer = _gatherDuration;
        }

        public override bool Perform(float deltaTime)
        {
            if (Target is IWorldObjectForAI t)
            {
                Vector3 dir = t.GetWorldTransform().position - _selfTransform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                    _selfTransform.rotation = Quaternion.LookRotation(dir);
            }

            _gatherTimer -= deltaTime;
            if (_gatherTimer <= 0)
            {
                Debug.Log($"Gathered resource from {Target}!");
                // Logic to add resource to inventory or destroy resource object
                return true;
            }

            return false;
        }
    }
}