using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
   public class AttackAction : ActionBase
    {
        private float _attackTimer;
        private float _attackCooldown = 1.0f;
        private int _damage = 10;

        public AttackAction(
            Transform transform, 
            float attackTimer, 
            float attackCooldown, 
            int damage) : base(transform)
        {
            _attackTimer = attackTimer;
            _attackCooldown = attackCooldown;
            _damage = damage;
            
            ActionName = "Attack Enemy";
            Cost = 10.0f;
        }

        protected override void DefinePreconditionsAndEffects()
        {
            _preconditions.Clear();
            _effects.Clear();

            if (Target != null)
            {
                _preconditions.Add(new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby, 
                    "Enemy", 
                    true, 
                    Target
                ));

                _preconditions.Add(new GoalCondition(
                    GlobalKeys.ConditionTag.Has,
                    GlobalKeys.Tool.MeleeWeapon,
                    true
                ));

                _effects.Add(new GoalCondition(
                    GlobalKeys.ConditionTag.Nearby,
                    "Enemy",
                    false,
                    Target
                ));
            }
        }

        public override void OnStart()
        {
            _attackTimer = 0;
        }

        public override bool Perform(float deltaTime)
        {
            if (Target is IWorldObjectForAI t)
            {
                Vector3 dir = t.GetWorldTransform().position - Transform.position;
                dir.y = 0;
                if (dir != Vector3.zero)
                    Transform.rotation = Quaternion.LookRotation(dir);
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