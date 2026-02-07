using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
   public class AttackAction : ActionBase
   {
        private float _attackTimer;
        private readonly int _damage;

        private readonly Transform _selfTransform;
        private readonly float _attackCooldown;

        public AttackAction(
            Transform transform, 
            IObjectForAI target, 
            float attackCooldown, 
            int damage) : base(target)
        {
            _selfTransform = transform;
            _attackCooldown = attackCooldown;
            _damage = damage;
        }
        
        public override void OnStart()
        {
            _attackTimer = 0;
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

            _attackTimer -= deltaTime;
            if (_attackTimer <= 0)
            {
                Debug.Log($"Attacking {Target}!");
                // ApplyDamage(_target); 
                
                _attackTimer = _attackCooldown;
                
                bool enemyIsDead = CheckIfEnemyDead(Target); 
                
                if (enemyIsDead)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfEnemyDead(IObjectForAI target)
        {
            return Random.value > 0.8f;
        }
    }
}