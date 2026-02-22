using System.Collections.Generic;
using AI.Goal;
using AI.Knowledge;
using UnityEngine;

namespace AI.Actions.Implementation
{
    public class GatherResourceActionStrategy : ActionStrategy
    {
        private readonly Transform _selfTransform;
        private readonly float _gatherDuration;

        public GatherResourceActionStrategy(Transform selfTransform, float gatherDuration)
        {
            _selfTransform = selfTransform;
            _gatherDuration = gatherDuration;
        }

        public override float GetCost(IObjectForAI target)
        {
            return 2.0f;
        }

        public override IEnumerable<GoalCondition> GetPreconditions(IObjectForAI target)
        {
            var preconditions = new List<GoalCondition>();
            foreach (var tag in target.GetTags())
            {
                preconditions.Add(new GoalCondition(GlobalKeys.ConditionTag.Nearby, tag, true, target));
                
                string requiredTool = null;
                if (tag == GlobalKeys.WorldObject.Tree)
                {
                    requiredTool = GlobalKeys.Tool.Woodcutter;
                }
                else if (tag == GlobalKeys.WorldObject.GoldOre)
                {
                    requiredTool = GlobalKeys.Tool.Pickaxe;
                }

                if (requiredTool != null)
                {
                    preconditions.Add(new GoalCondition(GlobalKeys.ConditionTag.Has, requiredTool, true));
                }
            }
            return preconditions;
        }

        public override IEnumerable<GoalCondition> GetEffects(IObjectForAI target)
        {
            var effects = new List<GoalCondition>();
            foreach (var tag in target.GetTags())
            {
                string resourceType = null;
                if (tag == GlobalKeys.WorldObject.Tree)
                {
                    resourceType = GlobalKeys.Resources.Wood;
                }
                else if (tag == GlobalKeys.WorldObject.GoldOre)
                {
                    resourceType = GlobalKeys.Resources.Gold;
                }

                if (resourceType != null)
                {
                    effects.Add(new GoalCondition(GlobalKeys.ConditionTag.Has, resourceType, true));
                }
            }
            return effects;
        }

        public override ActionBase CreateAction(IObjectForAI target)
        {
            return new GatherResourceAction(_selfTransform, target, _gatherDuration);
        }
    }
}