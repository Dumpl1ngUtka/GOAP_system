using System.Collections.Generic;
using System.Linq;
using AI.Knowledge;

namespace AI.Goal.Implementation
{
    public class HealAllyGoal : GoalBase
    {
        private IObjectForAI _targetAlly;

        public HealAllyGoal(IObjectForAI targetAlly = null)
        {
            _targetAlly = targetAlly;
        }
        
        public override IEnumerable<GoalCondition> GetDesiredState()
        {
            if (_targetAlly == null)
                return Enumerable.Empty<GoalCondition>();

            return new[]
            {
                new GoalCondition(
                    GlobalKeys.ConditionTag.IsHigh,
                    GlobalKeys.Resources.Health,
                    mustExist: true,
                    specificObject: _targetAlly)
            };
        }

        public override float GetPriority(IEnumerable<Fact> knowledgeBase)
        {
            _targetAlly ??= FindTarget(knowledgeBase);

            if (_targetAlly == null)
                return 0f;

            bool isLow = knowledgeBase.Any(f => 
                f.Target == _targetAlly && 
                f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health));
            
            if (isLow)
                return 100f;

            bool isHigh = knowledgeBase.Any(f => 
                f.Target == _targetAlly && 
                f.ConditionTag == GlobalKeys.ConditionTag.IsHigh && 
                f.ObjectTags.Contains(GlobalKeys.Resources.Health));
            
            if (!isHigh)
                return 50f;

            return 0f;
        }

        private IObjectForAI FindTarget(IEnumerable<Fact> knowledgeBase)
        {
            List<IObjectForAI> allies = knowledgeBase
                .Where(f => f.ObjectTags.Contains(GlobalKeys.WorldObject.Ally) && f.Target != null)
                .Select(f => f.Target)
                .Distinct()
                .ToList();

            if (!allies.Any())
                return null;

            IObjectForAI lowHealthAlly = allies.FirstOrDefault(ally => 
                knowledgeBase.Any(f => f.Target == ally && 
                                       f.ConditionTag == GlobalKeys.ConditionTag.IsLow && 
                                       f.ObjectTags.Contains(GlobalKeys.Resources.Health)));
            
            if (lowHealthAlly != null)
                return lowHealthAlly;

            IObjectForAI notHighHealthAlly = allies.FirstOrDefault(ally => 
                !knowledgeBase.Any(f => f.Target == ally && 
                                        f.ConditionTag == GlobalKeys.ConditionTag.IsHigh && 
                                        f.ObjectTags.Contains(GlobalKeys.Resources.Health)));
            
            return notHighHealthAlly ?? allies.FirstOrDefault();
        }
    }
}