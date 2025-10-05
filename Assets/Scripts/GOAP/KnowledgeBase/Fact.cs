namespace GOAP.KnowledgeBase
{
    public class Fact
    {
        public readonly FactTag Tag;
        public readonly IObjectForFact Object;

        public Fact(FactTag tag, IObjectForFact obj = null)
        {
            Tag = tag;
            Object = obj;
        }
    }
}