namespace GOAP
{
    public readonly struct Effect
    {
        public enum Type
        {
            Add,
            Remove,
        }
        
        public readonly Type EffectType;
        public readonly Belief Belief;   

        public Effect(Belief belief, Type type)
        {
            Belief = belief;
            EffectType = type;
        }
    }
}