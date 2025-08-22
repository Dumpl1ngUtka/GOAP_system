namespace Unit
{
    public interface IHealth
    {
        bool IsHealthLow { get; }
        float CurrentHealth { get; }
    }
}