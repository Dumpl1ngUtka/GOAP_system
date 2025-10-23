using System.Collections.Generic;
using GOAP.KnowledgeBase;
using OwnSystems.DamageSystem;
using UnityEngine;

namespace GOAP.Action
{
    public class AttackAction : GoapAction
    {
        private const float AttackTime = 1f;
        private const ushort Damage = 10;

        private IDamageable _target;
        private float _timer;
        
        public override string Name => "Attack";
        public override float Cost => 1;
        
        public override void OnEnter()
        {
            base.OnEnter();
            if (Target == null)
                IsFailed = true;
            if (Target is IDamageable damageableTarget) 
                _target = damageableTarget;
        }

        public override void Perform()
        {
            if ((Object)_target == null)
            {
                IsDone = true;
                return;
            }
            _timer += Time.deltaTime;
            if (_timer >= AttackTime)
            {
                _target.ApplyDamage(Damage);
                _timer = 0f;
            }
        }

        public override void OnExit()
        {
        }

        public override IEnumerable<FactWithCondition> GetEffects()
        {
            return new[]
            {
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.Nearby).WithAdditionObjectForFactTags(ObjectForFactTag.Enemy)),
            };
        }

        public override IEnumerable<Fact> GetPreconditions()
        {
            return new[]
            {
                new Fact(FactTag.Nearby).WithAdditionObjectForFactTags(ObjectForFactTag.Enemy)
            };
        }

        public override IGoapAction Clone() => new AttackAction();
    }
}