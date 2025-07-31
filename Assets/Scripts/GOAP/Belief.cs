namespace GOAP
{
    public struct Belief
    {
        public readonly string Name;
        public readonly int Count;

        public Belief(string name, int count = 1)
        {
            Name = name;
            Count = count;
        }
    }
}