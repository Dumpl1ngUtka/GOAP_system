namespace GOAP
{
    public class TakeItem : Action
    {
        protected override Condition[] Conditions { get; } = new Condition[] { };
        
        protected override Effect[] Effects { get; }
        
        public override void Do()
        {
            throw new System.NotImplementedException();
        }
    }
}