using System.Collections.Generic;
using GOAP.KnowledgeBase;
using Unit;
using UnityEngine;

namespace GOAP.Action
{
    public class HealAction : GoapAction
    {
        private const float HealTime = 2f;

        private IHealth _agent;
        private float _timer;

        public override string Name => "Heal";
        public override float Cost => 1;

        public HealAction(IHealth health)
        {
            _agent = health;
        }

        public override void OnEnter()
        {
            _timer = 0;
        }

        public override void Perform()
        {
            if (_timer >= HealTime)
            {
                _agent.ApplyHealing(20);
                _timer = 0;
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }

        public override void OnExit()
        {
        }

        public override IEnumerable<FactWithCondition> GetEffects() =>
            new[]
            {
                new FactWithCondition(FactCondition.Exclude, new Fact(FactTag.IsLowHealth)),
            };

        public override IEnumerable<Fact> GetPreconditions() => new List<Fact>();
    }
}