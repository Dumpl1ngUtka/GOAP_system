using AI.Knowledge;

namespace AI.Goal
{
    public class GoalCondition
    {
        public string ConditionTag { get; }
        public string ObjectTag { get; }
        public IObjectForAI SpecificObject { get; } 
        public bool MustExist { get; } 

        public GoalCondition(
            string conditionTag, 
            string objectTag,
            bool mustExist, 
            IObjectForAI specificObject = null)
        {
            ConditionTag = conditionTag;
            ObjectTag = objectTag;
            MustExist = mustExist;
            SpecificObject = specificObject;
        }
    }
}