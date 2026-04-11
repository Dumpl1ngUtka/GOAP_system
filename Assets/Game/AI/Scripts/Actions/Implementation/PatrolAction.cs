using AI.Knowledge;
using Units.Mover;
using UnityEngine;
using UnityEngine.AI; // Если захотите использовать проверку по NavMesh

namespace AI.Actions.Implementation
{
    public class PatrolAction : ActionBase
    {
        private readonly AgentMover _mover;
        private readonly Transform _center;
        private readonly float _radius;

        public PatrolAction(
            IObjectForAI target, 
            AgentMover mover, 
            Transform center, 
            float radius) : base(target)
        {
            _mover = mover;
            _center = center;
            _radius = radius;
        }

        public override void OnStart()
        {
            Vector2 randomCircle = Random.insideUnitCircle * _radius;
            
            Vector3 randomPos = _center.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            Debug.Log($"[RandomPatrol] Выбрана случайная точка: {randomPos}");
            
            _mover.SetTargetPosition(randomPos);
        }

        public override bool Perform(float deltaTime)
        {
            return !_mover.IsMoving; 
        }

        public override void OnStop()
        {
            _mover.Stop();
        }
    }
}